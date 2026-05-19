import { api } from '../lib/api';
import type { ApiResponse, Paginated, Task } from '../types';

export const taskService = {
  getByProject: async (projectId: string, params: Record<string, string | number | undefined>) => {
    const { data } = await api.get<ApiResponse<Paginated<Task>>>(`/tasks/project/${projectId}`, { params });
    return data.data!;
  },
  create: async (body: object) => {
    const { data } = await api.post<ApiResponse<Task>>('/tasks', body);
    return data.data!;
  },
  update: async (id: string, body: object) => {
    const { data } = await api.put<ApiResponse<Task>>(`/tasks/${id}`, body);
    return data.data!;
  },
  updateStatus: async (id: string, status: string) => {
    const { data } = await api.patch<ApiResponse<Task>>(`/tasks/${id}/status`, { status });
    return data.data!;
  },
  delete: async (id: string) => {
    await api.delete(`/tasks/${id}`);
  },
  uploadAttachment: async (id: string, file: File) => {
    const form = new FormData();
    form.append('file', file);
    const { data } = await api.post<ApiResponse<Task>>(`/tasks/${id}/attachment`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return data.data!;
  },
};
