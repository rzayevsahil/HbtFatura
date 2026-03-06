export class IbanFormatter {
    /**
     * Formats a raw string into TR IBAN format: TRXX XXXX XXXX XXXX XXXX XXXX XX
     * @param value Raw IBAN string
     * @returns Formatted IBAN string
     */
    static format(value: string): string {
        let cleaned = value.toUpperCase().replace(/[^A-Z0-9]/g, '');

        // Always start with TR
        if (cleaned.length > 0 && !cleaned.startsWith('TR')) {
            if (cleaned.startsWith('T')) cleaned = 'TR' + cleaned.substring(1);
            else cleaned = 'TR' + cleaned;
        } else if (cleaned.length === 0) {
            cleaned = 'TR';
        }

        // Keep only TR and numbers after prefix
        const prefix = cleaned.substring(0, 2);
        const rest = cleaned.substring(2).replace(/[^0-9]/g, '');
        cleaned = prefix + rest;

        // Limit to 26 characters (TR + 24 digits)
        if (cleaned.length > 26) cleaned = cleaned.substring(0, 26);

        // Add spaces every 4 characters
        let formatted = '';
        for (let i = 0; i < cleaned.length; i++) {
            if (i > 0 && i % 4 === 0) formatted += ' ';
            formatted += cleaned[i];
        }
        return formatted;
    }

    /**
     * Universal handler for (input) events on IBAN fields
     * @param event The input event
     * @returns The formatted string
     */
    static handleInput(event: Event): string {
        const input = event.target as HTMLInputElement;
        const formatted = this.format(input.value);
        input.value = formatted;
        return formatted;
    }
}
