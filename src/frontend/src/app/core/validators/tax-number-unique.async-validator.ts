import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { Observable, of, timer } from 'rxjs';
import { catchError, map, switchMap, take } from 'rxjs/operators';

import { TaxNumberValidationService } from '../services/tax-number-validation.service';

export type TaxNumberCheckMode = 'customer' | 'company';

export interface TaxNumberUniqueValidatorOptions {
  excludeCustomerId?: () => string | null | undefined;
  firmId?: () => string | null | undefined;
}

/**
 * Sunucu tarafı TC/VKN tekilliği. Cari formu için mode=customer, şirket ayarları için mode=company.
 */
export function taxNumberUniqueAsyncValidator(
  svc: TaxNumberValidationService,
  mode: TaxNumberCheckMode,
  opts: TaxNumberUniqueValidatorOptions = {}
): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    const value = control.value;
    if (value == null || String(value).trim() === '') {
      return of(null);
    }
    return timer(400).pipe(
      take(1),
      switchMap(() => {
        const body: {
          value: string;
          mode: TaxNumberCheckMode;
          excludeCustomerId?: string;
          firmId?: string;
        } = { value: String(value).trim(), mode };
        const excl = opts.excludeCustomerId?.();
        if (excl) body.excludeCustomerId = excl;
        const fid = opts.firmId?.();
        if (fid) body.firmId = fid;
        return svc.check(body);
      }),
      map((r) => {
        if (!r.isValidFormat) return null;
        return r.isUnique
          ? null
          : { taxNumberUnique: r.message ?? 'Bu numara zaten kayıtlı.' };
      }),
      catchError(() => of(null))
    );
  };
}
