import { useQuery } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { FolderKanban, CheckCircle2, Clock, ListTodo } from 'lucide-react';
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from 'recharts';
import { dashboardService } from '../services/dashboardService';
import { CardSkeleton } from '../components/Skeleton';

const COLORS = ['#6366f1', '#f59e0b', '#10b981'];

export default function DashboardPage() {
  const { data, isLoading } = useQuery({ queryKey: ['dashboard-stats'], queryFn: dashboardService.getStats });

  if (isLoading) return <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">{[1,2,3,4].map(i => <CardSkeleton key={i} />)}</div>;

  const stats = data!;
  const chartData = [
    { name: 'Completed', value: stats.completedTasks },
    { name: 'In Progress', value: stats.inProgressTasks },
    { name: 'Pending', value: stats.pendingTasks },
  ];

  const cards = [
    { label: 'Projects', value: stats.totalProjects, icon: FolderKanban, color: 'bg-blue-500' },
    { label: 'Total Tasks', value: stats.totalTasks, icon: ListTodo, color: 'bg-brand-600' },
    { label: 'Completed', value: stats.completedTasks, icon: CheckCircle2, color: 'bg-emerald-500' },
    { label: 'In Progress', value: stats.inProgressTasks, icon: Clock, color: 'bg-amber-500' },
  ];

  return (
    <div className="space-y-8">
      <div>
        <h1 className="text-2xl font-bold">Dashboard</h1>
        <p className="text-slate-500">Overview of your projects and tasks</p>
      </div>

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

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="card">
          <h2 className="mb-4 font-semibold">Task Distribution</h2>
          <ResponsiveContainer width="100%" height={250}>
            <PieChart>
              <Pie data={chartData} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={90} label>
                {chartData.map((_, i) => <Cell key={i} fill={COLORS[i % COLORS.length]} />)}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </div>

        <div className="card">
          <h2 className="mb-4 font-semibold">Recent Tasks</h2>
          {stats.recentTasks.length === 0 ? (
            <p className="text-center text-slate-500 py-8">No tasks yet</p>
          ) : (
            <ul className="space-y-3">
              {stats.recentTasks.map((task) => (
                <li key={task.id} className="flex items-center justify-between rounded-lg border border-slate-100 p-3 dark:border-slate-800">
                  <div>
                    <p className="font-medium">{task.title}</p>
                    <p className="text-xs text-slate-500">{task.projectName}</p>
                  </div>
                  <span className="rounded-full bg-slate-100 px-2 py-1 text-xs dark:bg-slate-800">{task.status}</span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}
