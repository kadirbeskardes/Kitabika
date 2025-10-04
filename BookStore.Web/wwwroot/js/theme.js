(function () {
    const themeToggle = document.getElementById('theme-toggle');
    try {
        // Prefer saved theme, otherwise fall back to light. Respects explicit localStorage value.
        let currentTheme = localStorage.getItem('theme');

        if (!currentTheme) {
            // default to light theme (orange tones)
            currentTheme = 'light';
            localStorage.setItem('theme', currentTheme);
        }

        document.documentElement.setAttribute('data-theme', currentTheme);

        if (themeToggle) {
            const icon = themeToggle.querySelector('i');
            if (icon) {
                if (currentTheme === 'dark') {
                    icon.classList.remove('bi-sun-fill');
                    icon.classList.add('bi-moon-fill');
                } else {
                    icon.classList.remove('bi-moon-fill');
                    icon.classList.add('bi-sun-fill');
                }
            }

            themeToggle.addEventListener('click', () => {
                let theme = document.documentElement.getAttribute('data-theme');
                const icon = themeToggle.querySelector('i');
                if (theme === 'dark') {
                    document.documentElement.setAttribute('data-theme', 'light');
                    localStorage.setItem('theme', 'light');
                    if (icon) {
                        icon.classList.remove('bi-moon-fill');
                        icon.classList.add('bi-sun-fill');
                    }
                } else {
                    document.documentElement.setAttribute('data-theme', 'dark');
                    localStorage.setItem('theme', 'dark');
                    if (icon) {
                        icon.classList.remove('bi-sun-fill');
                        icon.classList.add('bi-moon-fill');
                    }
                }
            });
        }
    }
    catch (e) {
        // fail silently in environments where DOM/localStorage may be unavailable
        console.warn('Theme initialization failed', e);
    }
})();
