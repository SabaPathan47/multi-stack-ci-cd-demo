package com.example.taskmanager.controller;

import com.example.taskmanager.model.Task;
import com.example.taskmanager.service.TaskService;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.servlet.WebMvcTest;
import org.springframework.boot.test.mock.mockito.MockBean;
import org.springframework.test.web.servlet.MockMvc;

import java.util.List;

import static org.mockito.Mockito.when;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.*;

@WebMvcTest(TaskController.class)
class TaskControllerTest {

    @Autowired
    private MockMvc mockMvc;

    @MockBean
    private TaskService service;

    @Autowired
    private ObjectMapper objectMapper;

    @Test
    void getAll_returnsOk() throws Exception {
        Task task = new Task("Demo", "desc");
        task.setId(1L);
        when(service.getAll()).thenReturn(List.of(task));

        mockMvc.perform(get("/api/tasks"))
                .andExpect(status().isOk())
                .andExpect(jsonPath("$[0].title").value("Demo"));
    }

    @Test
    void create_returnsCreated_whenValid() throws Exception {
        Task task = new Task("New task", "desc");
        task.setId(2L);
        when(service.create(org.mockito.ArgumentMatchers.any())).thenReturn(task);

        mockMvc.perform(post("/api/tasks")
                        .contentType("application/json")
                        .content(objectMapper.writeValueAsString(task)))
                .andExpect(status().isCreated())
                .andExpect(jsonPath("$.id").value(2));
    }

    @Test
    void create_returnsBadRequest_whenTitleMissing() throws Exception {
        Task invalid = new Task("", "desc");

        mockMvc.perform(post("/api/tasks")
                        .contentType("application/json")
                        .content(objectMapper.writeValueAsString(invalid)))
                .andExpect(status().isBadRequest());
    }
}
