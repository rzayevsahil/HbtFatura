export class PhoneFormatter {
    static format(value: string): string {
        let cleaned = value.replace(/[^0-9]/g, '');
        if (cleaned.length > 11) cleaned = cleaned.substring(0, 11);

        // Ensure it starts with 0 if there's any content
        if (cleaned.length > 0 && !cleaned.startsWith('0')) {
            cleaned = '0' + cleaned;
        } else if (cleaned.length === 0) {
            // If user clears everything, we could either leave empty or keep '0'
            // But based on current logic, we keep '0' if there was input
            // Let's stick to the current logic used in components for consistency
        }

        let formatted = '';
        for (let i = 0; i < cleaned.length; i++) {
            if (i === 4 || i === 7 || i === 9) formatted += ' ';
            formatted += cleaned[i];
        }
        return formatted.trim();
    }

    /**
     * Universal handler for (input) events on phone fields
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
