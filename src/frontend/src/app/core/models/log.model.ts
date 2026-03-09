import { LogLevel } from './common.model';

export interface LogEntry {
    id: string;
    timestamp: string;
    level: LogLevel;
    message: string;
    action?: string;
    module?: string;
    userId?: string;
    userFullName?: string;
    ipAddress?: string;
    details?: string;
}
