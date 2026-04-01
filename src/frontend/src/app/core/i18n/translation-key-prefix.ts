/** ngx-translate anahtarından ilk segment (örn. <code>invoices.pageTitle</code> → <code>invoices</code>). */
export function translationKeyPrefix(key: string): string {
  if (!key) return '__flat';
  const i = key.indexOf('.');
  return i < 0 ? '__flat' : key.slice(0, i);
}
