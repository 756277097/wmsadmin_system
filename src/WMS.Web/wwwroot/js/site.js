// 全局工具函数

// 显示提示消息
function showMessage(message, type = 'info') {
    const alertClass = {
        'success': 'alert-success',
        'error': 'alert-danger',
        'warning': 'alert-warning',
        'info': 'alert-info'
    }[type] || 'alert-info';

    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;

    const contentArea = document.getElementById('contentArea');
    if (contentArea) {
        contentArea.insertAdjacentHTML('afterbegin', alertHtml);
        
        // 3秒后自动关闭
        setTimeout(() => {
            const alert = contentArea.querySelector('.alert');
            if (alert) {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            }
        }, 3000);
    }
}

// AJAX请求封装
async function ajaxRequest(url, options = {}) {
    const defaultOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
        }
    };

    const mergedOptions = { ...defaultOptions, ...options };

    try {
        const response = await fetch(url, mergedOptions);
        const data = await response.json();
        return { success: response.ok, data };
    } catch (error) {
        console.error('AJAX请求失败:', error);
        return { success: false, error: error.message };
    }
}

// 页面加载完成后执行
document.addEventListener('DOMContentLoaded', function() {
    // 初始化菜单
    if (typeof initMenu !== 'undefined') {
        initMenu();
    }

    // 初始化权限控制
    if (typeof initPermission !== 'undefined') {
        initPermission();
    }

    // 初始化侧边栏收缩功能
    initSidebarToggle();
});

// 侧边栏收缩/展开功能
function initSidebarToggle() {
    const sidebar = document.getElementById('sidebar');
    const toggleBtn = document.getElementById('sidebarToggle');
    const mainContent = document.querySelector('main');
    
    if (!sidebar || !toggleBtn) return;

    // 从本地存储读取状态
    const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
    if (isCollapsed) {
        sidebar.classList.add('collapsed');
        if (mainContent) {
            mainContent.classList.add('sidebar-collapsed');
        }
    }

    // 点击切换
    toggleBtn.addEventListener('click', function(e) {
        e.preventDefault();
        e.stopPropagation();
        
        sidebar.classList.toggle('collapsed');
        const collapsed = sidebar.classList.contains('collapsed');
        
        // 动态调整主内容区域
        if (mainContent) {
            if (collapsed) {
                mainContent.classList.add('sidebar-collapsed');
            } else {
                mainContent.classList.remove('sidebar-collapsed');
            }
        }
        
        // 保存状态到本地存储
        localStorage.setItem('sidebarCollapsed', collapsed.toString());
    });
}

