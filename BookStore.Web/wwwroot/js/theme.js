const themeToggle = document.getElementById('theme-toggle');
const currentTheme = localStorage.getItem('theme');

if (currentTheme) {
    document.documentElement.setAttribute('data-theme', currentTheme);

    if (currentTheme === 'dark') {
        themeToggle.querySelector('i').classList.remove('bi-sun-fill');
        themeToggle.querySelector('i').classList.add('bi-moon-fill');
    }
}

themeToggle.addEventListener('click', () => {
    let theme = document.documentElement.getAttribute('data-theme');
    if (theme === 'dark') {
        document.documentElement.setAttribute('data-theme', 'light');
        localStorage.setItem('theme', 'light');
        themeToggle.querySelector('i').classList.remove('bi-moon-fill');
        themeToggle.querySelector('i').classList.add('bi-sun-fill');
    } else {
        document.documentElement.setAttribute('data-theme', 'dark');
        localStorage.setItem('theme', 'dark');
        themeToggle.querySelector('i').classList.remove('bi-sun-fill');
        themeToggle.querySelector('i').classList.add('bi-moon-fill');
    }
});
