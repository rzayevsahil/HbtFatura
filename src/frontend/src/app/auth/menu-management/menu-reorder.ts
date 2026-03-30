import { MenuItem } from '../../core/models';

export interface MenuRowVm {
  menu: MenuItem;
  depth: number;
}

/** Öncü gezinme sırası + derinlik; sürükle-bırak sonrası geçerliliği kontrol eder. */
export function isValidPreorderDepths(rows: MenuRowVm[]): boolean {
  const stack: number[] = [];
  for (const row of rows) {
    const d = row.depth;
    while (stack.length > 0 && stack[stack.length - 1] >= d) {
      stack.pop();
    }
    if (d === 0) {
      if (stack.length !== 0) return false;
    } else {
      if (stack.length === 0 || stack[stack.length - 1] !== d - 1) return false;
    }
    stack.push(d);
  }
  return true;
}

export function flattenMenuTreeWithDepth(menus: MenuItem[], depth = 0): MenuRowVm[] {
  const out: MenuRowVm[] = [];
  for (const m of menus) {
    out.push({ menu: m, depth });
    if (m.children?.length) {
      out.push(...flattenMenuTreeWithDepth(m.children, depth + 1));
    }
  }
  return out;
}

/** Satırları temiz bir ağaca dönüştürür (children/parentId sıfırdan kurulur). */
export function buildTreeFromPreorderRows(rows: MenuRowVm[]): MenuItem[] {
  if (rows.length === 0) return [];
  const idToMenu = new Map<string, MenuItem>();
  for (const row of rows) {
    idToMenu.set(row.menu.id, row.menu);
  }
  for (const m of idToMenu.values()) {
    m.children = [];
  }

  const roots: MenuItem[] = [];
  const stack: MenuRowVm[] = [];

  for (const row of rows) {
    while (stack.length > 0 && stack[stack.length - 1].depth >= row.depth) {
      stack.pop();
    }
    const parentVm = stack[stack.length - 1];
    const m = row.menu;
    if (!parentVm) {
      m.parentId = undefined;
      roots.push(m);
    } else {
      m.parentId = parentVm.menu.id;
      parentVm.menu.children.push(m);
    }
    stack.push(row);
  }

  assignSortOrderRecursive(roots);
  return roots;
}

function assignSortOrderRecursive(items: MenuItem[]) {
  items.forEach((m, i) => {
    m.sortOrder = i;
    if (m.children?.length) assignSortOrderRecursive(m.children);
  });
}

export function buildReorderPayload(roots: MenuItem[]): { id: string; parentId?: string; sortOrder: number }[] {
  const payload: { id: string; parentId?: string; sortOrder: number }[] = [];
  function walk(items: MenuItem[]) {
    for (const m of items) {
      payload.push({
        id: m.id,
        parentId: m.parentId,
        sortOrder: m.sortOrder
      });
      if (m.children?.length) walk(m.children);
    }
  }
  walk(roots);
  return payload;
}
