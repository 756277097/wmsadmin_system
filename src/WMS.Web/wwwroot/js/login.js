// 登录页交互脚本

document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');
    const passwordInput = document.getElementById('password');
    const passwordToggle = document.getElementById('passwordToggle');
    const eyeIcon = document.getElementById('eyeIcon');
    const loginBtn = document.getElementById('loginBtn');
    const errorAlert = document.getElementById('errorAlert');

    // 密码显示/隐藏切换
    if (passwordToggle && passwordInput) {
        passwordToggle.addEventListener('click', function() {
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);
            
            // 切换图标
            if (eyeIcon) {
                if (type === 'text') {
                    // 显示"眼睛关闭"图标
                    eyeIcon.innerHTML = '<path d="M13.359 11.238C15.06 9.72 16 8 16 8s-3-5.5-8-5.5a7 7 0 0 0-2.79.588l.77.771A6 6 0 0 1 8 3.5c2.12 0 3.879 1.168 5.168 2.457A13 13 0 0 1 14.828 8q-.086.13-.195.288c-.335.48-.83 1.12-1.465 1.755C11.879 11.332 10.119 12.5 8 12.5a7 7 0 0 1-2.808-.587l-.77.772A8 8 0 0 0 8 13.5c2.12 0 3.879-1.168 5.168-2.457A13 13 0 0 0 14.828 8"/><path d="M11.297 9.176a3 3 0 0 1-4.12-4.12l4.12 4.12"/><path d="M13.646 15.354l-12-12 .708-.708 12 12z"/>';
                } else {
                    // 显示"眼睛"图标
                    eyeIcon.innerHTML = '<path d="M16 8s-3-5.5-8-5.5S0 8 0 8s3 5.5 8 5.5S16 8 16 8M1.173 8a13 13 0 0 1 1.66-2.043C4.12 4.668 5.88 3.5 8 3.5s3.879 1.168 5.168 2.457A13 13 0 0 1 14.828 8q-.086.13-.195.288c-.335.48-.83 1.12-1.465 1.755C11.879 11.332 10.119 12.5 8 12.5s-3.879-1.168-5.168-2.457A13 13 0 0 1 1.172 8"/><path d="M8 5.5a2.5 2.5 0 1 0 0 5 2.5 2.5 0 0 0 0-5M4.5 8a3.5 3.5 0 1 1 7 0 3.5 3.5 0 0 1-7 0"/>';
                }
            }
        });
    }

    // 表单验证（只在提交时显示提示）
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            const userName = document.getElementById('userName');
            const password = document.getElementById('password');
            let isValid = true;

            // 清除之前的验证状态
            userName.classList.remove('is-invalid', 'is-valid');
            password.classList.remove('is-invalid', 'is-valid');

            // 验证用户名
            if (!userName.value.trim()) {
                userName.classList.add('is-invalid');
                isValid = false;
            } else {
                userName.classList.add('is-valid');
            }

            // 验证密码
            if (!password.value.trim()) {
                password.classList.add('is-invalid');
                isValid = false;
            } else {
                password.classList.add('is-valid');
            }

            if (!isValid) {
                e.preventDefault();
                e.stopPropagation();
                return false;
            }

            // 显示加载状态
            if (loginBtn) {
                loginBtn.disabled = true;
                loginBtn.querySelector('.btn-text').classList.add('d-none');
                loginBtn.querySelector('.btn-spinner').classList.remove('d-none');
            }
        });
    }

    // 输入框输入时清除错误状态（但不显示提示）
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.addEventListener('input', function() {
            // 只在已经显示错误的情况下才清除
            if (this.classList.contains('is-invalid')) {
                this.classList.remove('is-invalid');
            }
            // 清除成功状态
            if (this.classList.contains('is-valid')) {
                this.classList.remove('is-valid');
            }
        });

        input.addEventListener('focus', function() {
            // 获得焦点时清除错误状态
            if (this.classList.contains('is-invalid')) {
                this.classList.remove('is-invalid');
            }
        });
    });

    // 自动关闭错误提示（5秒后）
    if (errorAlert) {
        setTimeout(function() {
            const bsAlert = new bootstrap.Alert(errorAlert);
            bsAlert.close();
        }, 5000);
    }

    // Enter 键提交
    document.addEventListener('keypress', function(e) {
        if (e.key === 'Enter' && loginForm) {
            loginForm.dispatchEvent(new Event('submit'));
        }
    });
});

