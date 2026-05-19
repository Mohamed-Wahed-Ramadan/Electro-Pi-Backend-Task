export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors: string[];
}

export interface AuthUser {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
}

export interface Project {
  id: string;
  name: string;
  description: string;
  ownerUserId: string;
  ownerName: string;
  coverImageUrl?: string;
  createdAt: string;
  updatedAt?: string;
  taskCount: number;
}

export interface Task {
  id: string;
  title: string;
  description: string;
  status: 'Pending' | 'InProgress' | 'Completed';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  dueDate?: string;
  projectId: string;
  assignedUserId?: string;
  assignedUserName?: string;
  attachmentUrl?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface Paginated<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface DashboardStats {
  totalProjects: number;
  totalTasks: number;
  completedTasks: number;
  inProgressTasks: number;
  pendingTasks: number;
  recentTasks: {
    id: string;
    title: string;
    projectName: string;
    status: string;
    priority: string;
    dueDate?: string;
  }[];
}

export interface AdminStats {
  totalUsers: number;
  totalProjects: number;
  totalTasks: number;
  activeUsers: number;
}
