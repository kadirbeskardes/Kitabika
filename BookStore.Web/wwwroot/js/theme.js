class ThemeManager {
    constructor() {
        this.currentTheme = this.getStoredTheme() || 'light';
        this.init();
    }

    init() {
        this.applyTheme(this.currentTheme);
        this.createThemeToggle();
        this.bindEvents();
    }

    getStoredTheme() {
        return localStorage.getItem('theme');
    }

    setStoredTheme(theme) {
        localStorage.setItem('theme', theme);
    }

    applyTheme(theme) {
        if (theme === 'dark') {
            document.documentElement.setAttribute('data-theme', 'dark');
        } else {
            document.documentElement.removeAttribute('data-theme');
        }
        this.currentTheme = theme;
        this.setStoredTheme(theme);
        this.updateToggleIcon();
    }

    toggleTheme() {
        const newTheme = this.currentTheme === 'light' ? 'dark' : 'light';
        this.applyTheme(newTheme);
        
        // Optional: Show toast notification
        this.showThemeChangeNotification(newTheme);
    }

    createThemeToggle() {
        const toggleButton = document.createElement('button');
        toggleButton.id = 'theme-toggle';
        toggleButton.className = 'theme-toggle';
        toggleButton.setAttribute('aria-label', 'Toggle theme');
        toggleButton.innerHTML = '<i class="bi bi-sun-fill"></i>';
        
        document.body.appendChild(toggleButton);
    }

    updateToggleIcon() {
        const toggleButton = document.getElementById('theme-toggle');
        if (toggleButton) {
            const icon = toggleButton.querySelector('i');
            if (this.currentTheme === 'dark') {
                icon.className = 'bi bi-moon-fill';
            } else {
                icon.className = 'bi bi-sun-fill';
            }
        }
    }

    bindEvents() {
        document.addEventListener('click', (e) => {
            if (e.target.closest('#theme-toggle')) {
                this.toggleTheme();
            }
        });

        // Listen for system theme changes
        if (window.matchMedia) {
            const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
            mediaQuery.addEventListener('change', (e) => {
                if (!this.getStoredTheme()) {
                    this.applyTheme(e.matches ? 'dark' : 'light');
                }
            });
        }
    }

    showThemeChangeNotification(theme) {
        // Create a simple toast notification
        const toast = document.createElement('div');
        toast.className = 'toast-notification';
        toast.innerHTML = `
            <div class="toast-content">
                <i class="bi bi-${theme === 'dark' ? 'moon' : 'sun'}-fill me-2"></i>
                ${theme === 'dark' ? 'Karanlık' : 'Aydınlık'} tema aktif
            </div>
        `;
        
        // Add toast styles
        toast.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background-color: var(--card-bg);
            color: var(--text-primary);
            padding: 12px 20px;
            border-radius: 8px;
            box-shadow: 0 4px 12px var(--shadow-lg);
            z-index: 9999;
            transform: translateX(100%);
            transition: transform 0.3s ease;
            border: 1px solid var(--border-color);
        `;
        
        document.body.appendChild(toast);
        
        // Animate in
        setTimeout(() => {
            toast.style.transform = 'translateX(0)';
        }, 100);
        
        // Remove after 3 seconds
        setTimeout(() => {
            toast.style.transform = 'translateX(100%)';
            setTimeout(() => {
                document.body.removeChild(toast);
            }, 300);
        }, 3000);
    }

    // Public method to get current theme
    getCurrentTheme() {
        return this.currentTheme;
    }

    // Public method to set theme programmatically
    setTheme(theme) {
        if (theme === 'light' || theme === 'dark') {
            this.applyTheme(theme);
        }
    }
}

// Initialize theme manager when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    window.themeManager = new ThemeManager();
});

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ThemeManager;
}