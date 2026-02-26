import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/**
 * Turkish VKN (10 digits) and TCKN (11 digits with checksum) validation.
 * Empty value is considered valid (use Validators.required separately if needed).
 */
export function taxNumberValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const v = control.value as string | null | undefined;
    if (v == null || String(v).trim() === '') return null;
    const s = String(v).trim().replace(/\s/g, '');
    if (!/^\d+$/.test(s)) return { taxNumber: 'Sadece rakam giriniz.' };
    if (s.length === 10) {
      return null; // VKN: 10 digits, no checksum in spec
    }
    if (s.length === 11) {
      if (s[0] === '0') return { taxNumber: 'TCKN ilk hane 0 olamaz.' };
      const d = s.split('').map(Number);
      const odds = d[0] + d[2] + d[4] + d[6] + d[8];
      const evens = d[1] + d[3] + d[5] + d[7];
      const tenth = ((7 * odds - evens) % 10 + 10) % 10;
      if (d[9] !== tenth) return { taxNumber: 'TCKN 10. hane hatalı.' };
      const sum10 = d.slice(0, 10).reduce((a, b) => a + b, 0);
      const eleventh = sum10 % 10;
      if (d[10] !== eleventh) return { taxNumber: 'TCKN 11. hane hatalı.' };
      return null;
    }
    return { taxNumber: 'VKN 10, TCKN 11 haneli olmalıdır.' };
  };
}
