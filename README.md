# Multi-Stack Reusable CI/CD Demo

Two real applications — a Spring Boot service and an ASP.NET Core service —
sharing **one** reusable GitHub Actions workflow that covers build, unit
test, an NVIDIA-LLM-backed AI review agent, security scanning, container
image build/publish, and staged deployment (dev → staging → production).

```
.
├── .github/
│   ├── agents/
│   │   ├── pr-review-agent.md          # system prompt for the AI code reviewer
│   │   └── security-review-agent.md    # system prompt for the AI scan summarizer
│   ├── scripts/
│   │   └── nvidia_agent_review.py      # calls NVIDIA NIM API, posts PR comment
│   └── workflows/
│       ├── reusable-ci-cd.yml          # the shared pipeline (workflow_call)
│       ├── java-app-ci-cd.yml          # caller for the Spring Boot app
│       └── dotnet-app-ci-cd.yml        # caller for the .NET app
└── apps/
    ├── java-task-manager/              # Spring Boot 3 + JPA + H2, Maven
    └── dotnet-product-catalog/         # ASP.NET Core 8 Web API
```

## Pipeline stages (identical shape for both apps)

1. **build-test** — compile + run unit tests (JUnit5/Mockito for Java,
   xUnit/Moq for .NET), publish test report artifacts.
2. **ai-review** — on pull requests only: sends the PR diff to an NVIDIA
   NIM-hosted LLM using the persona in `pr-review-agent.md`, posts findings
   as a PR comment.
3. **security-scan** — Trivy filesystem scan of dependencies (SARIF uploaded
   to GitHub code scanning, fails the job on CRITICAL/HIGH), then an AI
   summary of the scan posted to the PR.
4. **build-publish-image** — multi-stage Docker build, pushes to
   `ghcr.io/<owner>/<repo>/<app-name>`, then a Trivy **image** scan gates the
   job on CRITICAL/HIGH vulnerabilities in the final image.
5. **deploy-dev / deploy-staging / deploy-production** — staged rollout:
   - `feature/**` pushes → `dev` environment
   - `main` pushes → `staging`, then `production` (gated by a GitHub
     Environment approval rule)

Both `push` and `pull_request` events trigger the pipeline for `main` and
`feature/**` branches; deploy jobs only run on `push` (never on PRs).

## One-time repo setup

**Secrets** (Settings → Secrets and variables → Actions):
- `NVIDIA_API_KEY` — API key from [build.nvidia.com](https://build.nvidia.com)
  for the NIM model endpoint. If omitted, the AI review/summary steps log a
  notice and exit successfully rather than failing the pipeline.

**Variables** (optional):
- `NVIDIA_MODEL` — override the default `meta/llama-3.1-70b-instruct` model.

**Environments** (Settings → Environments) — create `dev`, `staging`,
`production`. Add required reviewers to `production` to get a manual
approval gate before the last deploy job runs.

**Permissions** — `GITHUB_TOKEN` needs `packages: write` to push to GHCR and
`pull-requests: write` for the AI agent to comment; both are already set in
`reusable-ci-cd.yml`'s top-level `permissions:` block, but org-level policy
must allow it too (Settings → Actions → General → Workflow permissions).

## Replacing the placeholder deploy steps

The `deploy-dev` / `deploy-staging` / `deploy-production` jobs currently
`echo` what they'd do. Swap the `run:` step for your real target, e.g.:

```yaml
- name: Deploy to staging
  run: |
    kubectl --context staging set image deployment/${{ inputs.app_name }} \
      app=ghcr.io/${{ inputs.image_name }}:${{ needs.build-publish-image.outputs.image_tag }}
```
