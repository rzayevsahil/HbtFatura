import { TranslateService } from '@ngx-translate/core';
import { MenuItem } from '../models';

/** API `routerLink` → `menu.*` i18n anahtarı */
export const MENU_LINK_I18N_KEYS: Record<string, string> = {
  '/dashboard': 'menu.dashboard',
  '/invoices': 'menu.invoices',
  '/gib-simulation/inbox': 'menu.gibInbox',
  '/orders': 'menu.orders',
  '/delivery-notes': 'menu.deliveryNotes',
  '/products': 'menu.products',
  '/customers': 'menu.customers',
  '/main-account-codes': 'menu.mainAccountCodes',
  '/payments': 'menu.payments',
  '/cash-registers': 'menu.cashRegisters',
  '/bank-accounts': 'menu.bankAccounts',
  '/cheques': 'menu.cheques',
  '/reports': 'menu.reports',
  '/lookups': 'menu.lookups',
  '/permissions': 'menu.permissions',
  '/menus': 'menu.menus',
  '/material-icons': 'menu.materialIcons',
  '/translations-admin': 'menu.translationsAdmin',
  '/firms': 'menu.firms',
  '/employees': 'menu.employees',
  '/logs': 'menu.logs',
  '/company/profile': 'menu.companyProfile'
};

/** Seed’deki Türkçe etiket → `menu.*` (routerLink yoksa veya eşleşmezse) */
export const MENU_LABEL_FALLBACK_I18N_KEYS: Record<string, string> = {
  Dashboard: 'menu.dashboard',
  Faturalar: 'menu.invoices',
  'GİB Kutusu (simülasyon)': 'menu.gibInbox',
  Siparişler: 'menu.orders',
  İrsaliyeler: 'menu.deliveryNotes',
  Ürünler: 'menu.products',
  'Cari Kartlar': 'menu.customers',
  'Hesap Kodları': 'menu.mainAccountCodes',
  'Tahsilat / Ödeme': 'menu.payments',
  'Kasa Yönetimi': 'menu.cashRegisters',
  'Banka Yönetimi': 'menu.bankAccounts',
  'Çek / Senet': 'menu.cheques',
  Raporlar: 'menu.reports',
  'Sistem Tanımları': 'menu.lookups',
  'Rol ve Yetki Yönetimi': 'menu.permissions',
  'Menü Yönetimi': 'menu.menus',
  'Material İkonları': 'menu.materialIcons',
  'Firma Yönetimi': 'menu.firms',
  'Personel Yönetimi': 'menu.employees',
  'Sistem Logları': 'menu.logs',
  'Şirket Profili': 'menu.companyProfile'
};

/** Sol menü ve menü yönetim tablosu için ortak görünen ad. */
export function getMenuDisplayLabel(translate: TranslateService, item: MenuItem): string {
  const link = (item.routerLink || '').trim();
  const linkKey = link ? MENU_LINK_I18N_KEYS[link] : undefined;
  if (linkKey) {
    const t = translate.instant(linkKey);
    if (t !== linkKey) {
      return t;
    }
  }
  const fb = item.label ? MENU_LABEL_FALLBACK_I18N_KEYS[item.label] : undefined;
  if (fb) {
    const t = translate.instant(fb);
    if (t !== fb) {
      return t;
    }
  }
  return item.label;
}
