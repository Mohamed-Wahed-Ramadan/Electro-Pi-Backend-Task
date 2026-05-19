import { api } from '../lib/api';
import type { AdminStats, ApiResponse, DashboardStats } from '../types';

export const dashboardService = {
  getStats: async () => {
    const { data } = await api.get<ApiResponse<DashboardStats>>('/dashboard/stats');
    return data.data!;
  },
  getAdminStats: async () => {
    const { data } = await api.get<ApiResponse<AdminStats>>('/dashboard/admin');
    return data.data!;
  },
};
