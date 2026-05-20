export const toPublicAssetUrl = (url?: string | null): string | undefined => {
  if (!url) return undefined;

  const rewritten = url
    .replace('http://minio:9000', 'http://localhost:9000')
    .replace('https://minio:9000', 'http://localhost:9000')
    .replace('http://pm-minio:9000', 'http://localhost:9000')
    .replace('https://pm-minio:9000', 'http://localhost:9000');

  // Old signed URLs may have stale signatures; files are public now, so use clean object URL.
  if (rewritten.startsWith('http://localhost:9000/')) {
    const queryIndex = rewritten.indexOf('?');
    return queryIndex >= 0 ? rewritten.slice(0, queryIndex) : rewritten;
  }

  return rewritten;
};
