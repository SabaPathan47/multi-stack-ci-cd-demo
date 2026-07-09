#!/usr/bin/env python3
"""
Calls an NVIDIA NIM-hosted LLM using one of the agent definitions in
.github/agents/*.md as the system prompt, then posts the result as a PR
comment via the GitHub CLI.

Usage:
  python nvidia_agent_review.py --mode review   --input diff.txt
  python nvidia_agent_review.py --mode security --input trivy-report.json

Required env vars:
  NVIDIA_API_KEY   - NVIDIA NIM API key (repo/org secret)
  GH_TOKEN         - GitHub token with pull-requests:write (provided by Actions)
  PR_NUMBER        - pull request number to comment on
  GITHUB_REPOSITORY - "owner/repo" (set automatically by Actions)
Optional:
  NVIDIA_MODEL     - defaults to meta/llama-3.1-70b-instruct
"""
import argparse
import json
import os
import subprocess
import sys
import urllib.request

NVIDIA_ENDPOINT = "https://integrate.api.nvidia.com/v1/chat/completions"
DEFAULT_MODEL = "meta/llama-3.1-70b-instruct"

AGENT_FILES = {
    "review": ".github/agents/pr-review-agent.md",
    "security": ".github/agents/security-review-agent.md",
}


def load_agent_prompt(mode: str) -> str:
    path = AGENT_FILES[mode]
    with open(path, "r", encoding="utf-8") as f:
        return f.read()


def call_nvidia(system_prompt: str, user_content: str, model: str, api_key: str) -> str:
    payload = {
        "model": model,
        "messages": [
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_content[:60000]},  # guard against huge payloads
        ],
        "temperature": 0.2,
        "max_tokens": 1024,
    }
    req = urllib.request.Request(
        NVIDIA_ENDPOINT,
        data=json.dumps(payload).encode("utf-8"),
        headers={
            "Authorization": f"Bearer {api_key}",
            "Content-Type": "application/json",
            "Accept": "application/json",
        },
        method="POST",
    )
    try:
        with urllib.request.urlopen(req, timeout=90) as resp:
            data = json.loads(resp.read().decode("utf-8"))
    except Exception as exc:  # network or API error shouldn't fail the whole pipeline
        return f"_AI agent call failed: {exc}_"

    try:
        return data["choices"][0]["message"]["content"]
    except (KeyError, IndexError):
        return f"_AI agent returned an unexpected response: {json.dumps(data)[:500]}_"


def post_pr_comment(body: str, pr_number: str, app_label: str):
    marker = f"<!-- ai-agent:{app_label} -->"
    full_body = f"{marker}\n{body}"
    subprocess.run(
        ["gh", "pr", "comment", pr_number, "--body", full_body],
        check=True,
    )


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--mode", choices=["review", "security"], required=True)
    parser.add_argument("--input", required=True, help="Path to diff or scan report file")
    parser.add_argument("--app-label", default="app", help="Label used in the PR comment, e.g. java-task-manager")
    args = parser.parse_args()

    api_key = os.environ.get("NVIDIA_API_KEY")
    pr_number = os.environ.get("PR_NUMBER")

    if not api_key:
        print("NVIDIA_API_KEY not set — skipping AI agent step.", file=sys.stderr)
        sys.exit(0)  # don't fail the pipeline just because the agent isn't configured

    if not pr_number:
        print("PR_NUMBER not set — not a pull_request event, skipping.", file=sys.stderr)
        sys.exit(0)

    model = os.environ.get("NVIDIA_MODEL", DEFAULT_MODEL)

    with open(args.input, "r", encoding="utf-8", errors="ignore") as f:
        content = f.read()

    if not content.strip():
        print("Input file empty, nothing to review.", file=sys.stderr)
        sys.exit(0)

    system_prompt = load_agent_prompt(args.mode)
    result = call_nvidia(system_prompt, content, model, api_key)
    post_pr_comment(result, pr_number, f"{args.app_label}-{args.mode}")
    print("Posted AI agent comment to PR", pr_number)


if __name__ == "__main__":
    main()
