import { Pipe, PipeTransform } from '@angular/core';
import { resolveMaterialIconLigature } from './material-icons.registry';

/** `Material+Icons` ligature metni → webfontta geçerli önizleme adı (alias dahil). */
@Pipe({
  name: 'materialIconLigature',
  standalone: true
})
export class MaterialIconLigaturePipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    return resolveMaterialIconLigature(value);
  }
}
