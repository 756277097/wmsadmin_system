// 角色权限管理页面

let allMenus = [];
let selectedPermissions = [];
let rolePermissions = []; // 编辑时的已有权限

// 初始化权限树
async function initPermissionTree() {
    try {
        const response = await fetch('/Menu/GetTree');
        allMenus = await response.json();
        renderPermissionTree();
    } catch (error) {
        console.error('加载菜单树失败:', error);
    }
}

// 渲染权限树
function renderPermissionTree() {
    const treeContainer = document.getElementById('permissionTree');
    if (!treeContainer) return;

    treeContainer.innerHTML = '';

    if (allMenus.length === 0) {
        treeContainer.innerHTML = '<p class="text-muted">暂无菜单</p>';
        return;
    }

    const ul = document.createElement('ul');
    ul.className = 'permission-tree';

    allMenus.forEach(menu => {
        const li = createPermissionTreeNode(menu);
        ul.appendChild(li);
    });

    treeContainer.appendChild(ul);

    // 如果有已选权限，恢复选中状态
    if (rolePermissions && rolePermissions.length > 0) {
        restoreSelectedPermissions();
    }
}

// 创建权限树节点
function createPermissionTreeNode(menu) {
    const li = document.createElement('li');

    // 菜单项
    const menuDiv = document.createElement('div');
    menuDiv.className = 'permission-item';

    const menuCheckbox = document.createElement('input');
    menuCheckbox.type = 'checkbox';
    menuCheckbox.className = 'form-check-input menu-checkbox';
    menuCheckbox.value = `menu_${menu.id}`;
    menuCheckbox.dataset.menuId = menu.id;
    menuCheckbox.dataset.permissionType = '0';
    menuCheckbox.id = `menu_checkbox_${menu.id}`;
    menuCheckbox.addEventListener('change', function() {
        handleMenuCheckboxChange(menu.id, this.checked);
    });

    const menuLabel = document.createElement('label');
    menuLabel.className = 'form-check-label';
    menuLabel.htmlFor = `menu_checkbox_${menu.id}`;
    menuLabel.innerHTML = `<strong>${menu.name}</strong> <span class="code-badge">${menu.code}</span>`;

    menuDiv.appendChild(menuCheckbox);
    menuDiv.appendChild(menuLabel);

    // 按钮列表
    if (menu.buttons && menu.buttons.length > 0) {
        const buttonContainer = document.createElement('div');
        buttonContainer.className = 'button-permissions-container';
        buttonContainer.style.marginTop = '0.5rem';
        buttonContainer.style.marginLeft = '1.5rem';
        buttonContainer.style.display = 'flex';
        buttonContainer.style.flexWrap = 'wrap';
        buttonContainer.style.gap = '0.5rem';

        menu.buttons.forEach(button => {
            const buttonDiv = document.createElement('div');
            buttonDiv.className = 'button-permission';

            const buttonCheckbox = document.createElement('input');
            buttonCheckbox.type = 'checkbox';
            buttonCheckbox.className = 'button-checkbox';
            buttonCheckbox.value = `button_${button.id}`;
            buttonCheckbox.dataset.menuId = menu.id;
            buttonCheckbox.dataset.buttonId = button.id;
            buttonCheckbox.dataset.permissionType = '1';
            buttonCheckbox.id = `button_checkbox_${button.id}`;
            buttonCheckbox.addEventListener('change', function() {
                handleButtonCheckboxChange(menu.id, button.id, this.checked);
            });

            const buttonLabel = document.createElement('label');
            buttonLabel.htmlFor = `button_checkbox_${button.id}`;
            buttonLabel.textContent = button.name;
            buttonLabel.style.cursor = 'pointer';
            buttonLabel.style.margin = '0';

            buttonDiv.appendChild(buttonCheckbox);
            buttonDiv.appendChild(buttonLabel);
            buttonContainer.appendChild(buttonDiv);
        });

        menuDiv.appendChild(buttonContainer);
    }

    li.appendChild(menuDiv);

    // 子菜单
    if (menu.children && menu.children.length > 0) {
        const childrenUl = document.createElement('ul');
        childrenUl.className = 'permission-children';

        menu.children.forEach(child => {
            const childLi = createPermissionTreeNode(child);
            childrenUl.appendChild(childLi);
        });

        li.appendChild(childrenUl);
    }

    return li;
}

// 处理菜单复选框变化
function handleMenuCheckboxChange(menuId, checked) {
    if (checked) {
        addPermission(menuId, null, 0);
    } else {
        removePermission(menuId, null, 0);
        // 取消选中该菜单下的所有按钮
        document.querySelectorAll(`[data-menu-id="${menuId}"].button-checkbox`).forEach(checkbox => {
            checkbox.checked = false;
            const buttonId = checkbox.dataset.buttonId;
            removePermission(menuId, parseInt(buttonId), 1);
        });
    }
}

// 处理按钮复选框变化
function handleButtonCheckboxChange(menuId, buttonId, checked) {
    if (checked) {
        addPermission(menuId, buttonId, 1);
    } else {
        removePermission(menuId, buttonId, 1);
    }
}

// 添加权限
function addPermission(menuId, buttonId, permissionType) {
    const permission = {
        menuId: menuId,
        buttonId: buttonId,
        permissionType: permissionType
    };

    // 检查是否已存在
    const exists = selectedPermissions.some(p => 
        p.menuId === menuId && 
        p.buttonId === buttonId && 
        p.permissionType === permissionType
    );

    if (!exists) {
        selectedPermissions.push(permission);
    }
}

// 移除权限
function removePermission(menuId, buttonId, permissionType) {
    selectedPermissions = selectedPermissions.filter(p => 
        !(p.menuId === menuId && 
          p.buttonId === buttonId && 
          p.permissionType === permissionType)
    );
}

// 恢复已选权限
function restoreSelectedPermissions() {
    rolePermissions.forEach(permission => {
        if (permission.permissionType === 0) {
            // 菜单权限
            const checkbox = document.querySelector(`[data-menu-id="${permission.menuId}"].menu-checkbox`);
            if (checkbox) {
                checkbox.checked = true;
                addPermission(permission.menuId, null, 0);
            }
        } else {
            // 按钮权限
            const checkbox = document.querySelector(`[data-menu-id="${permission.menuId}"][data-button-id="${permission.buttonId}"].button-checkbox`);
            if (checkbox) {
                checkbox.checked = true;
                addPermission(permission.menuId, permission.buttonId, 1);
            }
        }
    });
}

// 获取选中的权限（用于表单提交）
function getSelectedPermissions() {
    return selectedPermissions;
}

// 在表单提交前，将权限数据添加到表单
document.addEventListener('DOMContentLoaded', function() {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            // 创建隐藏字段存储权限数据
            const existingInput = form.querySelector('input[name="permissionsJson"]');
            if (existingInput) {
                existingInput.value = JSON.stringify(selectedPermissions);
            } else {
                const permissionsInput = document.createElement('input');
                permissionsInput.type = 'hidden';
                permissionsInput.name = 'permissionsJson';
                permissionsInput.value = JSON.stringify(selectedPermissions);
                form.appendChild(permissionsInput);
            }
        });
    });

    // 初始化权限树
    if (document.getElementById('permissionTree')) {
        initPermissionTree();
    }
});

