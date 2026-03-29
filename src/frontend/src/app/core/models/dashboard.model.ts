export interface DashboardStat {
    label: string;
    value: string;
    icon: string;
    color: string;
    trend?: string;
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
