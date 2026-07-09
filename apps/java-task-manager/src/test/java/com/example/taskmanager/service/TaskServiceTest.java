package com.example.taskmanager.service;

import com.example.taskmanager.exception.TaskNotFoundException;
import com.example.taskmanager.model.Task;
import com.example.taskmanager.repository.TaskRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.util.List;
import java.util.Optional;

import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatThrownBy;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class TaskServiceTest {

    @Mock
    private TaskRepository repository;

    @InjectMocks
    private TaskService service;

    private Task task;

    @BeforeEach
    void setUp() {
        task = new Task("Write tests", "Cover the service layer");
        task.setId(1L);
    }

    @Test
    void getAll_returnsAllTasks() {
        when(repository.findAll()).thenReturn(List.of(task));
        List<Task> result = service.getAll();
        assertThat(result).hasSize(1).contains(task);
    }

    @Test
    void getById_returnsTask_whenExists() {
        when(repository.findById(1L)).thenReturn(Optional.of(task));
        Task result = service.getById(1L);
        assertThat(result.getTitle()).isEqualTo("Write tests");
    }

    @Test
    void getById_throws_whenNotFound() {
        when(repository.findById(99L)).thenReturn(Optional.empty());
        assertThatThrownBy(() -> service.getById(99L))
                .isInstanceOf(TaskNotFoundException.class);
    }

    @Test
    void create_savesAndReturnsTask() {
        when(repository.save(any(Task.class))).thenReturn(task);
        Task result = service.create(new Task("Write tests", "Cover the service layer"));
        assertThat(result.getId()).isEqualTo(1L);
        verify(repository, times(1)).save(any(Task.class));
    }

    @Test
    void delete_removesTask_whenExists() {
        when(repository.findById(1L)).thenReturn(Optional.of(task));
        service.delete(1L);
        verify(repository, times(1)).delete(task);
    }
}
