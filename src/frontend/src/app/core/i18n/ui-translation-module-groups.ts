/**
 * Arayüz çevirileri anahtarlarının ilk segmenti (JSON kökü).
 * Okunur başlıklar `translationGroups.<id>` i18n anahtarlarından gelir — tr.json / en.json ile aynı küme tutulmalı.
 * `__flat` noktasız anahtarlar; listede sonda gösterilir.
 */
export const UI_TRANSLATION_MODULE_GROUP_IDS: readonly string[] = [
  'app',
  'auth',
  'notFound',
  'dashboard',
  'products',
  'common',
  'iconPicker',
  'invoices',
  'orders',
  'deliveryNotes',
  'customers',
  'menu',
  'mainAccountCodes',
  'payments',
  'cashRegisters',
  'bankAccountsPage',
  'chequesPage',
  'gibInbox',
  'reportsPage',
  'employeesPage',
  'permissionManagement',
  'menuManagement',
  'materialIconsPage',
  'translationsPage',
  'firmsPage',
  'firmForm',
  'logsPage',
  'lookupsPage',
  'firmProfile',
  '__flat'
] as const;
