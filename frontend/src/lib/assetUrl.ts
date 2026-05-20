export const toPublicAssetUrl = (url?: string | null): string | undefined => {
  if (!url) return undefined;

  return url
    .replace('http://minio:9000', 'http://localhost:9000')
    .replace('http://pm-minio:9000', 'http://localhost:9000');
};
