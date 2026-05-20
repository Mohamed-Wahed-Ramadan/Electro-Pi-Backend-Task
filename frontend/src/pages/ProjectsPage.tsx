import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Pencil, Plus, Search, Trash2 } from 'lucide-react';
import toast from 'react-hot-toast';
import { projectService } from '../services/projectService';
import Modal from '../components/Modal';
import { CardSkeleton } from '../components/Skeleton';
import { toPublicAssetUrl } from '../lib/assetUrl';

export default function ProjectsPage() {
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [modalOpen, setModalOpen] = useState(false);
  const [editModalOpen, setEditModalOpen] = useState(false);
  const [editingProjectId, setEditingProjectId] = useState<string | null>(null);
  const [form, setForm] = useState({ name: '', description: '' });
  const [editForm, setEditForm] = useState({ name: '', description: '' });
  const queryClient = useQueryClient();

  const { data, isLoading } = useQuery({
    queryKey: ['projects', page, search],
    queryFn: () => projectService.getAll({ pageNumber: page, pageSize: 9, search }),
  });

  const createMutation = useMutation({
    mutationFn: () => projectService.create(form),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      toast.success('Project created');
      setModalOpen(false);
      setForm({ name: '', description: '' });
    },
    onError: () => toast.error('Failed to create project'),
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => projectService.delete(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      toast.success('Project deleted');
    },
    onError: () => toast.error('Failed to delete project'),
  });

  const updateMutation = useMutation({
    mutationFn: () => projectService.update(editingProjectId!, editForm),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['projects'] });
      toast.success('Project updated');
      setEditModalOpen(false);
      setEditingProjectId(null);
      setEditForm({ name: '', description: '' });
    },
    onError: () => toast.error('Failed to update project'),
  });

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold">Projects</h1>
          <p className="text-slate-500">Manage your project portfolio</p>
        </div>
        <button className="btn-primary" onClick={() => setModalOpen(true)}><Plus size={18} className="mr-2" /> New Project</button>
      </div>

      <div className="relative max-w-md">
        <Search className="absolute left-3 top-2.5 text-slate-400" size={18} />
        <input className="input pl-10" placeholder="Search projects..." value={search} onChange={(e) => { setSearch(e.target.value); setPage(1); }} />
      </div>

      {isLoading ? (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">{[1,2,3].map(i => <CardSkeleton key={i} />)}</div>
      ) : data?.items.length === 0 ? (
        <div className="card py-16 text-center text-slate-500">No projects found. Create your first project!</div>
      ) : (
        <>
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
            {data?.items.map((project, i) => (
              <motion.div key={project.id} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.03 }}>
                <div className="card transition hover:shadow-md hover:border-brand-200 dark:hover:border-brand-700">
                  <div className="mb-2 flex justify-end">
                    <button
                      type="button"
                      className="rounded-lg p-2 text-slate-500 hover:bg-slate-100 dark:hover:bg-slate-800"
                      onClick={() => {
                        setEditingProjectId(project.id);
                        setEditForm({ name: project.name, description: project.description });
                        setEditModalOpen(true);
                      }}
                    >
                      <Pencil size={16} />
                    </button>
                    <button
                      type="button"
                      className="rounded-lg p-2 text-red-500 hover:bg-red-50 dark:hover:bg-red-900/20"
                      onClick={() => {
                        if (confirm('Delete this project?')) {
                          deleteMutation.mutate(project.id);
                        }
                      }}
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                  <Link to={`/projects/${project.id}`} className="block">
                    {project.coverImageUrl && <img src={toPublicAssetUrl(project.coverImageUrl)} alt="" className="mb-3 h-32 w-full rounded-lg object-cover" />}
                  <h3 className="font-semibold">{project.name}</h3>
                  <p className="mt-1 line-clamp-2 text-sm text-slate-500">{project.description}</p>
                  <div className="mt-3 flex items-center justify-between text-xs text-slate-400">
                    <span>{project.taskCount} tasks</span>
                    <span>{project.ownerName}</span>
                  </div>
                  </Link>
                </div>
              </motion.div>
            ))}
          </div>
          {data && data.totalPages > 1 && (
            <div className="flex justify-center gap-2">
              <button className="btn-secondary" disabled={!data.hasPreviousPage} onClick={() => setPage(p => p - 1)}>Previous</button>
              <span className="flex items-center px-3 text-sm">Page {data.pageNumber} of {data.totalPages}</span>
              <button className="btn-secondary" disabled={!data.hasNextPage} onClick={() => setPage(p => p + 1)}>Next</button>
            </div>
          )}
        </>
      )}

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title="Create Project">
        <form onSubmit={(e) => { e.preventDefault(); createMutation.mutate(); }} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium">Name</label>
            <input className="input" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} required />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Description</label>
            <textarea className="input min-h-[100px]" value={form.description} onChange={(e) => setForm({ ...form, description: e.target.value })} />
          </div>
          <button type="submit" className="btn-primary w-full" disabled={createMutation.isPending}>Create</button>
        </form>
      </Modal>

      <Modal open={editModalOpen} onClose={() => setEditModalOpen(false)} title="Update Project">
        <form onSubmit={(e) => { e.preventDefault(); updateMutation.mutate(); }} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium">Name</label>
            <input className="input" value={editForm.name} onChange={(e) => setEditForm({ ...editForm, name: e.target.value })} required />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium">Description</label>
            <textarea className="input min-h-[100px]" value={editForm.description} onChange={(e) => setEditForm({ ...editForm, description: e.target.value })} />
          </div>
          <button type="submit" className="btn-primary w-full" disabled={updateMutation.isPending}>Save Changes</button>
        </form>
      </Modal>
    </div>
  );
}
