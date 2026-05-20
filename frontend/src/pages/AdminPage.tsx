import { useQuery } from '@tanstack/react-query';
import { Navigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Users, FolderKanban, ListTodo, UserCheck } from 'lucide-react';
import { useAuthStore } from '../store/authStore';
import { dashboardService } from '../services/dashboardService';
import { CardSkeleton } from '../components/Skeleton';

export default function AdminPage() {
  const isAdmin = useAuthStore((s) => s.isAdmin);
  const { data, isLoading } = useQuery({
    queryKey: ['admin-stats'],
    queryFn: dashboardService.getAdminStats,
    enabled: isAdmin(),
  });

  if (!isAdmin()) return <Navigate to="/dashboard" />;

  const cards = data ? [
    { label: 'Total Users', value: data.totalUsers, icon: Users, color: 'bg-blue-500' },
    { label: 'Active Users', value: data.activeUsers, icon: UserCheck, color: 'bg-emerald-500' },
    { label: 'Total Projects', value: data.totalProjects, icon: FolderKanban, color: 'bg-brand-600' },
    { label: 'Total Tasks', value: data.totalTasks, icon: ListTodo, color: 'bg-amber-500' },
  ] : [];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Admin Dashboard</h1>
        <p className="text-slate-500">System-wide statistics and overview</p>
      </div>

      {isLoading ? (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">{[1,2,3,4].map(i => <CardSkeleton key={i} />)}</div>
      ) : (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {cards.map((card, i) => (
            <motion.div key={card.label} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.05 }} className="card flex items-center gap-4">
              <div className={`flex h-12 w-12 items-center justify-center rounded-xl text-white ${card.color}`}>
                <card.icon size={24} />
              </div>
              <div>
                <p className="text-sm text-slate-500">{card.label}</p>
                <p className="text-2xl font-bold">{card.value}</p>
              </div>
            </motion.div>
          ))}
        </div>
      )}
    </div>
  );
}
