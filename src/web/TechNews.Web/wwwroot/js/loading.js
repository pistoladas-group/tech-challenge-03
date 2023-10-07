const PageLoading = {
    show: () => {
        document.body.classList.add('page-loading');
        document.body.setAttribute('data-page-loading', "on");
    },
    hide: () => {
        document.body.classList.remove('page-loading');
        document.body.removeAttribute('data-page-loading');
    }
};