/**
 * Birim fiyat yanında gösterilecek para birimi metni (₺, $ vb. sembol yok).
 * TRY → TL; TL → TL; diğerleri büyük harf ISO kodu (USD, EUR, CHF, …).
 */
export function currencyDisplaySuffix(code: string | undefined | null): string {
  if (code == null || code.trim() === '') return 'TRY';
  const c = code.trim().toUpperCase();
  return c;
}
