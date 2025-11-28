// 权限控制
// 权限数据从Session中加载，每个页面都会从Session读取最新的权限数据

let userPermissions = [];

// 初始化权限（从Session中加载）
function initPermission() {
    // 优先从页面中获取权限数据（由后端从Session注入）
    const permissionDataElement = document.getElementById('permissionData');
    if (permissionDataElement) {
        try {
            const permissionText = permissionDataElement.textContent;
            userPermissions = JSON.parse(permissionText) || [];
            console.log('从Session加载权限数据:', userPermissions);
            console.log('权限数量:', userPermissions.length);
        } catch (e) {
            console.error('解析权限数据失败:', e);
            console.error('权限数据内容:', permissionDataElement.textContent);
            userPermissions = [];
        }
    } else {
        // 如果没有注入数据，尝试从API获取（API也会从Session读取）
        console.log('页面中未找到权限数据，尝试从API获取...');
        loadPermissionsFromApi();
        return; // loadPermissionsFromApi 会调用 controlButtons
    }

    // 控制按钮显示/隐藏
    controlButtons();
}

// 从API加载权限（API会从Session中读取权限数据）
async function loadPermissionsFromApi() {
    try {
        const response = await fetch('/Auth/GetUserPermissions');
        if (response.ok) {
            userPermissions = await response.json();
            console.log('从API加载权限数据（从Session读取）:', userPermissions);
            console.log('权限数量:', userPermissions.length);
            controlButtons();
        } else {
            console.error('获取权限失败，HTTP状态:', response.status);
            userPermissions = [];
            controlButtons();
        }
    } catch (error) {
        console.error('加载权限失败:', error);
        userPermissions = [];
        controlButtons();
    }
}

// 检查是否有权限
function hasPermission(permissionCode) {
    return userPermissions.includes(permissionCode);
}

// 控制按钮显示/隐藏
function controlButtons() {
    // 查找所有带data-permission属性的元素（包括按钮、链接等）
    const elements = document.querySelectorAll('[data-permission]');
    console.log(`找到 ${elements.length} 个需要权限控制的元素`);
    
    let hiddenCount = 0;
    let visibleCount = 0;
    
    elements.forEach(element => {
        const permissionCode = element.getAttribute('data-permission');
        if (!hasPermission(permissionCode)) {
            // 根据data-permission-action决定是隐藏还是禁用
            const action = element.getAttribute('data-permission-action') || 'hide';
            if (action === 'hide') {
                element.style.display = 'none';
                hiddenCount++;
                console.log(`隐藏元素（无权限）: ${permissionCode}`);
            } else if (action === 'disable') {
                element.disabled = true;
                element.classList.add('disabled');
                console.log(`禁用元素（无权限）: ${permissionCode}`);
            }
        } else {
            visibleCount++;
            console.log(`显示元素（有权限）: ${permissionCode}`);
        }
    });
    
    console.log(`权限控制完成 - 隐藏: ${hiddenCount}, 显示: ${visibleCount}`);
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

