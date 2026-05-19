import { api } from '../lib/api';
import type { ApiResponse, Paginated, Project } from '../types';

export const projectService = {
  getAll: async (params: Record<string, string | number | boolean>) => {
    const { data } = await api.get<ApiResponse<Paginated<Project>>>('/projects', { params });
    return data.data!;
  },
  getById: async (id: string) => {
    const { data } = await api.get<ApiResponse<Project>>(`/projects/${id}`);
    return data.data!;
  },
  create: async (body: { name: string; description: string }) => {
    const { data } = await api.post<ApiResponse<Project>>('/projects', body);
    return data.data!;
  },
  update: async (id: string, body: { name: string; description: string }) => {
    const { data } = await api.put<ApiResponse<Project>>(`/projects/${id}`, body);
    return data.data!;
  },
  delete: async (id: string) => {
    await api.delete(`/projects/${id}`);
  },
  uploadCover: async (id: string, file: File) => {
    const form = new FormData();
    form.append('file', file);
    const { data } = await api.post<ApiResponse<Project>>(`/projects/${id}/cover`, form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return data.data!;
  },
};
