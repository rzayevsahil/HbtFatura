/**
 * Query/state üzerinden gelen `returnUrl` için güvenli dahili yol (/path).
 * Open redirect ve `javascript:` vb. kaçınılır; yalnızca köke göre path kabul edilir.
 */
export function sanitizeInternalReturnUrl(raw: string | null | undefined): string | null {
  if (raw == null || typeof raw !== 'string') return null;
  const t = raw.trim();
  if (!t.startsWith('/') || t.startsWith('//') || /^(\\\\|\/\/|[a-zA-Z][a-zA-Z+.-]*:)/.test(t)) return null;
  return t;
}
