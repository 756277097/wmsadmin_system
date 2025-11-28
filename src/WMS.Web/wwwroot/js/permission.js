// 权限控制

let userPermissions = [];

// 初始化权限
function initPermission() {
    // 从页面中获取权限数据（由后端注入）
    const permissionDataElement = document.getElementById('permissionData');
    if (permissionDataElement) {
        try {
            userPermissions = JSON.parse(permissionDataElement.textContent) || [];
        } catch (e) {
            console.error('解析权限数据失败:', e);
            userPermissions = [];
        }
    } else {
        // 如果没有注入数据，尝试从API获取
        loadPermissionsFromApi();
    }

    // 控制按钮显示/隐藏
    controlButtons();
}

// 从API加载权限
async function loadPermissionsFromApi() {
    try {
        const response = await fetch('/Auth/GetUserPermissions');
        if (response.ok) {
            userPermissions = await response.json();
            controlButtons();
        }
    } catch (error) {
        console.error('加载权限失败:', error);
    }
}

// 检查是否有权限
function hasPermission(permissionCode) {
    return userPermissions.includes(permissionCode);
}

// 控制按钮显示/隐藏
function controlButtons() {
    // 查找所有带data-permission属性的按钮
    document.querySelectorAll('[data-permission]').forEach(button => {
        const permissionCode = button.getAttribute('data-permission');
        if (!hasPermission(permissionCode)) {
            // 根据data-permission-action决定是隐藏还是禁用
            const action = button.getAttribute('data-permission-action') || 'hide';
            if (action === 'hide') {
                button.style.display = 'none';
            } else if (action === 'disable') {
                button.disabled = true;
                button.classList.add('disabled');
            }
        }
    });
}

// 检查菜单权限（用于动态加载菜单时）
function hasMenuPermission(menuCode) {
    return hasPermission(menuCode);
}

// 检查按钮权限（用于动态创建按钮时）
function hasButtonPermission(buttonCode) {
    return hasPermission(buttonCode);
}

// 创建带权限控制的按钮
function createPermissionButton(buttonCode, buttonText, buttonClass = 'btn btn-primary', onClick) {
    if (!hasButtonPermission(buttonCode)) {
        return null;
    }

    const button = document.createElement('button');
    button.className = buttonClass;
    button.textContent = buttonText;
    button.setAttribute('data-permission', buttonCode);
    if (onClick) {
        button.addEventListener('click', onClick);
    }
    return button;
}

// 批量控制元素权限
function controlElementsByPermission(selector, permissionCode, action = 'hide') {
    document.querySelectorAll(selector).forEach(element => {
        if (!hasPermission(permissionCode)) {
            if (action === 'hide') {
                element.style.display = 'none';
            } else if (action === 'disable') {
                element.disabled = true;
                element.classList.add('disabled');
            }
        }
    });
}

