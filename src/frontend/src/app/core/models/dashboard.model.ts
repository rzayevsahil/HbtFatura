export interface DashboardStat {
    /** API çeviri anahtarı (örn. monthly_sales) */
    key?: string;
    label: string;
    value: string;
    icon: string;
    color: string;
    trend?: string;
    amount?: number | null;
    count?: number | null;
    trendCount?: number | null;
    trendKind?: string | null;
}

export interface RecentInvoice {
    id: string;
    customer: string;
    amount: string;
    date: string;
    status: string;
    /** InvoiceStatus enum; yoksa SuperAdmin firma satırı */
    statusCode?: number | null;
}

export interface RecentActivity {
    id: string;
    type: string;
    title: string;
    description: string;
    time: string;
}

export interface DashboardData {
    stats: DashboardStat[];
    recentInvoices: RecentInvoice[];
    activities: RecentActivity[];
}
