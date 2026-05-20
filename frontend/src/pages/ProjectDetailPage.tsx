import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { motion } from 'framer-motion';
import { ArrowLeft, Pencil, Upload, Trash2 } from 'lucide-react';
import toast from 'react-hot-toast';
import { projectService } from '../services/projectService';
import { taskService } from '../services/taskService';
import Modal from '../components/Modal';
import { CardSkeleton } from '../components/Skeleton';
import { toPublicAssetUrl } from '../lib/assetUrl';

const statuses = ['Pending', 'InProgress', 'Completed'];
const priorities = ['Low', 'Medium', 'High', 'Critical'];

export default function ProjectDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [taskModal, setTaskModal] = useState(false);
  const [projectEditModal, setProjectEditModal] = useState(false);
  const [taskEditModal, setTaskEditModal] = useState(false);
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [taskForm, setTaskForm] = useState({ title: '', description: '', priority: 'Medium', dueDate: '' });
  const [projectForm, setProjectForm] = useState({ name: '', description: '' });
  const [editingTaskId, setEditingTaskId] = useState<string | null>(null);
  const [taskEditForm, setTaskEditForm] = useState({ title: '', description: '', priority: 'Medium', dueDate: '' });

  const { data: project, isLoading: projectLoading } = useQuery({
    queryKey: ['project', id],
    queryFn: () => projectService.getById(id!),
    enabled: !!id,
  });

  const { data: tasks, isLoading: tasksLoading } = useQuery({
    queryKey: ['tasks', id, search, statusFilter],
    queryFn: () => taskService.getByProject(id!, { pageNumber: 1, pageSize: 50, search, status: statusFilter || undefined }),
    enabled: !!id,
  });

  const createTask = useMutation({
    mutationFn: () => taskService.create({
      title: taskForm.title,
      description: taskForm.description,
      priority: taskForm.priority,
      dueDate: taskForm.dueDate || null,
      projectId: id,
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks', id] });
      toast.success('Task created');
      setTaskModal(false);
      setTaskForm({ title: '', description: '', priority: 'Medium', dueDate: '' });
    },
    onError: () => toast.error('Failed to create task'),
  });

  const updateStatus = useMutation({
    mutationFn: ({ taskId, status }: { taskId: string; status: string }) => taskService.updateStatus(taskId, status),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['tasks', id] }),
  });

  const deleteTask = useMutation({
    mutationFn: (taskId: string) => taskService.delete(taskId),
    onSuccess: () => { queryClient.invalidateQueries({ queryKey: ['tasks', id] }); toast.success('Task deleted'); },
  });

  const updateProject = useMutation({
    mutationFn: () => projectService.update(id!, projectForm),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['project', id] });
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      toast.success('Project updated');
      setProjectEditModal(false);
    },
    onError: () => toast.error('Failed to update project'),
  });

  const deleteProject = useMutation({
    mutationFn: () => projectService.delete(id!),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      toast.success('Project deleted');
      navigate('/projects');
    },
    onError: () => toast.error('Failed to delete project'),
  });

  const updateTask = useMutation({
    mutationFn: () => taskService.update(editingTaskId!, {
      title: taskEditForm.title,
      description: taskEditForm.description,
      priority: taskEditForm.priority,
      dueDate: taskEditForm.dueDate || null,
      assignedUserId: null,
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['tasks', id] });
      toast.success('Task updated');
      setTaskEditModal(false);
      setEditingTaskId(null);
      setTaskEditForm({ title: '', description: '', priority: 'Medium', dueDate: '' });
    },
    onError: () => toast.error('Failed to update task'),
  });

  const uploadCover = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file || !id) return;
    try {
      await projectService.uploadCover(id, file);
      queryClient.invalidateQueries({ queryKey: ['project', id] });
      toast.success('Cover uploaded');
    } catch { toast.error('Upload failed'); }
  };

  const uploadAttachment = async (taskId: string, file: File) => {
    try {
      await taskService.uploadAttachment(taskId, file);
      queryClient.invalidateQueries({ queryKey: ['tasks', id] });
      toast.success('Attachment uploaded');
    } catch { toast.error('Upload failed'); }
  };

  if (projectLoading) return <CardSkeleton />;

  return (
    <div className="space-y-6">
      <button onClick={() => navigate('/projects')} className="flex items-center gap-2 text-sm text-slate-500 hover:text-brand-600">
        <ArrowLeft size={16} /> Back to Projects
      </button>

      <div className="card">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div>
            <h1 className="text-2xl font-bold">{project?.name}</h1>
            <p className="mt-1 text-slate-500">{project?.description}</p>
            <p className="mt-2 text-sm text-slate-400">Owner: {project?.ownerName}</p>
          </div>
          <div className="flex flex-wrap gap-2">
            <button
              className="btn-secondary"
              onClick={() => {
                setProjectForm({ name: project?.name ?? '', description: project?.description ?? '' });
                setProjectEditModal(true);
              }}
            >
              <Pencil size={16} className="mr-2" /> Edit Project
            </button>
            <label className="btn-secondary cursor-pointer">
              <Upload size={16} className="mr-2" /> Upload Cover
              <input type="file" accept="image/*" className="hidden" onChange={uploadCover} />
            </label>
            <button
              className="rounded-lg bg-red-600 px-4 py-2 text-white hover:bg-red-700"
              onClick={() => confirm('Delete this project?') && deleteProject.mutate()}
            >
              <Trash2 size={16} className="mr-2 inline" />
              Delete Project
            </button>
          </div>
        </div>
        {project?.coverImageUrl && <img src={toPublicAssetUrl(project.coverImageUrl)} alt="" className="mt-4 h-48 w-full rounded-lg object-cover" />}
      </div>

      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <h2 className="text-xl font-semibold">Tasks</h2>
        <button className="btn-primary" onClick={() => setTaskModal(true)}>Add Task</button>
      </div>

      <div className="flex flex-wrap gap-3">
        <input className="input max-w-xs" placeholder="Search tasks..." value={search} onChange={(e) => setSearch(e.target.value)} />
        <select className="input max-w-[160px]" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value)}>
          <option value="">All Statuses</option>
          {statuses.map(s => <option key={s} value={s}>{s}</option>)}
        </select>
      </div>

      {tasksLoading ? (
        <div className="space-y-3">{[1,2,3].map(i => <CardSkeleton key={i} />)}</div>
      ) : tasks?.items.length === 0 ? (
        <div className="card py-12 text-center text-slate-500">No tasks in this project</div>
      ) : (
        <div className="space-y-3">
          {tasks?.items.map((task, i) => (
            <motion.div key={task.id} initial={{ opacity: 0, x: -10 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: i * 0.02 }} className="card flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <h3 className="font-medium">{task.title}</h3>
                <p className="text-sm text-slate-500">{task.description}</p>
                <div className="mt-2 flex gap-2 text-xs">
                  <span className="rounded bg-slate-100 px-2 py-0.5 dark:bg-slate-800">{task.priority}</span>
                  {task.dueDate && <span className="text-slate-400">Due: {new Date(task.dueDate).toLocaleDateString()}</span>}
                </div>
                {task.attachmentUrl && <a href={toPublicAssetUrl(task.attachmentUrl)} target="_blank" rel="noreferrer" className="mt-1 block text-xs text-brand-600">View attachment</a>}
              </div>
              <div className="flex flex-wrap items-center gap-2">
                <select className="input max-w-[140px] py-1 text-sm" value={task.status} onChange={(e) => updateStatus.mutate({ taskId: task.id, status: e.target.value })}>
                  {statuses.map(s => <option key={s} value={s}>{s}</option>)}
                </select>
                <label className="btn-secondary cursor-pointer py-1 text-xs">
                  <Upload size={14} />
                  <input type="file" className="hidden" onChange={(e) => e.target.files?.[0] && uploadAttachment(task.id, e.target.files[0])} />
                </label>
                <button
                  className="rounded-lg p-2 text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800"
                  onClick={() => {
                    setEditingTaskId(task.id);
                    setTaskEditForm({
                      title: task.title,
                      description: task.description,
                      priority: task.priority,
                      dueDate: task.dueDate ? new Date(task.dueDate).toISOString().slice(0, 10) : '',
                    });
                    setTaskEditModal(true);
                  }}
                >
                  <Pencil size={16} />
                </button>
                <button className="rounded-lg p-2 text-red-500 hover:bg-red-50 dark:hover:bg-red-900/20" onClick={() => confirm('Delete task?') && deleteTask.mutate(task.id)}>
                  <Trash2 size={16} />
                </button>
              </div>
            </motion.div>
          ))}
        </div>
      )}

      <Modal open={taskModal} onClose={() => setTaskModal(false)} title="Create Task">
        <form onSubmit={(e) => { e.preventDefault(); createTask.mutate(); }} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium">Title</label>
            <input className="input" value={taskForm.title} onChange={(e) => setTaskForm({ ...taskForm, title: e.target.value })} required />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Description</label>
            <textarea className="input min-h-[80px]" value={taskForm.description} onChange={(e) => setTaskForm({ ...taskForm, description: e.target.value })} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="mb-1 block text-sm font-medium">Priority</label>
              <select className="input" value={taskForm.priority} onChange={(e) => setTaskForm({ ...taskForm, priority: e.target.value })}>
                {priorities.map(p => <option key={p} value={p}>{p}</option>)}
              </select>
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium">Due Date</label>
              <input className="input" type="date" value={taskForm.dueDate} onChange={(e) => setTaskForm({ ...taskForm, dueDate: e.target.value })} />
            </div>
          </div>
          <button type="submit" className="btn-primary w-full" disabled={createTask.isPending}>Create Task</button>
        </form>
      </Modal>

      <Modal open={projectEditModal} onClose={() => setProjectEditModal(false)} title="Update Project">
        <form onSubmit={(e) => { e.preventDefault(); updateProject.mutate(); }} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium">Name</label>
            <input className="input" value={projectForm.name} onChange={(e) => setProjectForm({ ...projectForm, name: e.target.value })} required />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Description</label>
            <textarea className="input min-h-[80px]" value={projectForm.description} onChange={(e) => setProjectForm({ ...projectForm, description: e.target.value })} />
          </div>
          <button type="submit" className="btn-primary w-full" disabled={updateProject.isPending}>Save Changes</button>
        </form>
      </Modal>

      <Modal open={taskEditModal} onClose={() => setTaskEditModal(false)} title="Update Task">
        <form onSubmit={(e) => { e.preventDefault(); updateTask.mutate(); }} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium">Title</label>
            <input className="input" value={taskEditForm.title} onChange={(e) => setTaskEditForm({ ...taskEditForm, title: e.target.value })} required />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Description</label>
            <textarea className="input min-h-[80px]" value={taskEditForm.description} onChange={(e) => setTaskEditForm({ ...taskEditForm, description: e.target.value })} />
          </div>
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="mb-1 block text-sm font-medium">Priority</label>
              <select className="input" value={taskEditForm.priority} onChange={(e) => setTaskEditForm({ ...taskEditForm, priority: e.target.value })}>
                {priorities.map(p => <option key={p} value={p}>{p}</option>)}
              </select>
            </div>
            <div>
              <label className="mb-1 block text-sm font-medium">Due Date</label>
              <input className="input" type="date" value={taskEditForm.dueDate} onChange={(e) => setTaskEditForm({ ...taskEditForm, dueDate: e.target.value })} />
            </div>
          </div>
          <button type="submit" className="btn-primary w-full" disabled={updateTask.isPending}>Save Changes</button>
        </form>
      </Modal>
    </div>
  );
}
