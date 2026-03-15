import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export interface TaxNumberValidatorOptions {
  /** TCKN için 10. ve 11. hane checksum doğrulaması yapılsın mı? VKN/TC alanında false kullanarak sadece 10/11 hane kabul edilir, algoritma bozulmaz. */
  validateTcknChecksum?: boolean;
}

/**
 * Turkish VKN (10 digits) and TCKN (11 digits) validation.
 * - 10 hane: VKN kabul (checksum yok).
 * - 11 hane: İlk hane 0 olamaz; validateTcknChecksum true ise 10. ve 11. hane TCKN algoritmasına göre doğrulanır.
 * Boş değer geçerlidir (required ayrı ekleyin).
 */
export function taxNumberValidator(options?: TaxNumberValidatorOptions): ValidatorFn {
  const validateTcknChecksum = options?.validateTcknChecksum !== false;
  return (control: AbstractControl): ValidationErrors | null => {
    const v = control.value as string | null | undefined;
    if (v == null || String(v).trim() === '') return null;
    const s = String(v).trim().replace(/\s/g, '');
    if (!/^\d+$/.test(s)) return { taxNumber: 'Sadece rakam giriniz.' };
    if (s.length > 11) return { taxNumber: 'En fazla 11 rakam girebilirsiniz.' };
    if (s.length === 10) return null; // VKN
    if (s.length === 11) {
      if (s[0] === '0') return { taxNumber: 'TCKN ilk hane 0 olamaz.' };
      if (!validateTcknChecksum) return null; // Sadece format: 11 hane, ilk ≠ 0
      const d = s.split('').map(Number);
      const odds = d[0] + d[2] + d[4] + d[6] + d[8];
      const evens = d[1] + d[3] + d[5] + d[7];
      const tenth = ((7 * odds - evens) % 10 + 10) % 10;
      if (d[9] !== tenth) return { taxNumber: 'TCKN 10. hane hatalı.' };
      const eleventh = d.slice(0, 10).reduce((a, b) => a + b, 0) % 10;
      if (d[10] !== eleventh) return { taxNumber: 'TCKN 11. hane hatalı.' };
      return null;
    }
    return { taxNumber: 'VKN 10, TCKN 11 haneli olmalıdır.' };
  };
}
