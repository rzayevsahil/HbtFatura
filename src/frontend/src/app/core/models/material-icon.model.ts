export interface MaterialIconDto {
  id: string;
  ligatureName: string;
  sortOrder: number;
  isActive: boolean;
}

export interface CreateMaterialIconRequest {
  ligatureName: string;
  sortOrder: number;
  isActive: boolean;
}
