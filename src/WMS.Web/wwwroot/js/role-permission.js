// 角色权限管理页面

let allMenus = [];
let selectedPermissions = [];
let rolePermissions = []; // 编辑时的已有权限

// 初始化权限树
async function initPermissionTree() {
    try {
        const response = await fetch('/Menu/GetTree');
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        allMenus = await response.json();
        console.log('加载的菜单数据:', allMenus);
        console.log('菜单数量:', allMenus.length);
        
        // 统计菜单和按钮数量
        let menuCount = 0;
        let buttonCount = 0;
        function countMenus(menus) {
            menus.forEach(menu => {
                menuCount++;
                if (menu.buttons && menu.buttons.length > 0) {
                    buttonCount += menu.buttons.length;
                }
                if (menu.children && menu.children.length > 0) {
                    countMenus(menu.children);
                }
            });
        }
        countMenus(allMenus);
        console.log(`菜单总数: ${menuCount}, 按钮总数: ${buttonCount}`);
        
        renderPermissionTree();
    } catch (error) {
        console.error('加载菜单树失败:', error);
        const treeContainer = document.getElementById('permissionTree');
        if (treeContainer) {
            treeContainer.innerHTML = '<p class="text-danger">加载菜单失败，请刷新页面重试</p>';
        }
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

    // 如果有已选权限，恢复选中状态（延迟执行确保DOM已完全渲染）
    if (rolePermissions && rolePermissions.length > 0) {
        setTimeout(() => {
            restoreSelectedPermissions();
        }, 100);
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

    // 按钮列表 - 显示所有按钮（包括禁用的）
    if (menu.buttons && menu.buttons.length > 0) {
        const buttonContainer = document.createElement('div');
        buttonContainer.className = 'button-permissions-container';
        buttonContainer.style.marginTop = '0.5rem';
        buttonContainer.style.marginLeft = '1.5rem';
        buttonContainer.style.display = 'flex';
        buttonContainer.style.flexWrap = 'wrap';
        buttonContainer.style.gap = '0.5rem';

        // 按排序字段排序按钮
        const sortedButtons = [...menu.buttons].sort((a, b) => (a.sort || 0) - (b.sort || 0));

        sortedButtons.forEach(button => {
            const buttonDiv = document.createElement('div');
            buttonDiv.className = 'button-permission';
            
            // 如果按钮被禁用，添加视觉提示
            if (!button.isEnabled) {
                buttonDiv.style.opacity = '0.6';
                buttonDiv.title = '此按钮已禁用';
            }

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
    // 确保 menuId 不为 null
    if (menuId === null || menuId === undefined) {
        console.warn('尝试添加权限时 menuId 为空');
        return;
    }

    const permission = {
        menuId: menuId,
        buttonId: buttonId || null, // 菜单权限时 buttonId 为 null
        permissionType: permissionType
    };

    // 检查是否已存在（使用严格比较）
    const exists = selectedPermissions.some(p => 
        p.menuId === menuId && 
        (p.buttonId === buttonId || (p.buttonId === null && buttonId === null)) && 
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
    console.log('开始恢复权限，权限数据:', rolePermissions);
    
    if (!rolePermissions || rolePermissions.length === 0) {
        console.log('没有权限数据需要恢复');
        // 即使没有权限，也要清空已选列表
        selectedPermissions = [];
        return;
    }

    // 清空已选权限列表，重新构建
    selectedPermissions = [];

    rolePermissions.forEach(permission => {
        // 确保使用正确的属性名（camelCase）
        const menuId = permission.menuId !== undefined ? permission.menuId : permission.MenuId;
        const buttonId = permission.buttonId !== undefined ? permission.buttonId : permission.ButtonId;
        const permissionType = permission.permissionType !== undefined ? permission.permissionType : permission.PermissionType;

        if (menuId === null || menuId === undefined) {
            console.warn('权限数据缺少MenuId:', permission);
            return;
        }

        if (permissionType === 0) {
            // 菜单权限
            const menuIdStr = String(menuId);
            const checkbox = document.querySelector(`[data-menu-id="${menuIdStr}"].menu-checkbox`);
            if (checkbox) {
                checkbox.checked = true;
                addPermission(menuId, null, 0);
                console.log(`✓ 恢复菜单权限: MenuId=${menuId}`);
            } else {
                console.warn(`✗ 未找到菜单复选框: MenuId=${menuId}`);
            }
        } else if (permissionType === 1) {
            // 按钮权限
            if (buttonId === null || buttonId === undefined) {
                console.warn('按钮权限缺少ButtonId:', permission);
                return;
            }
            const menuIdStr = String(menuId);
            const buttonIdStr = String(buttonId);
            const checkbox = document.querySelector(`[data-menu-id="${menuIdStr}"][data-button-id="${buttonIdStr}"].button-checkbox`);
            if (checkbox) {
                checkbox.checked = true;
                addPermission(menuId, buttonId, 1);
                console.log(`✓ 恢复按钮权限: MenuId=${menuId}, ButtonId=${buttonId}`);
            } else {
                console.warn(`✗ 未找到按钮复选框: MenuId=${menuId}, ButtonId=${buttonId}`);
            }
        } else {
            console.warn('未知的权限类型:', permissionType, permission);
        }
    });

    console.log('权限恢复完成，已选权限数量:', selectedPermissions.length);
    console.log('已选权限详情:', selectedPermissions);
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
            // 确保收集所有当前选中的权限
            collectCurrentPermissions();
            
            // 创建隐藏字段存储权限数据
            let existingInput = form.querySelector('input[name="permissionsJson"]');
            const permissionsJson = JSON.stringify(selectedPermissions);
            
            console.log('=== 表单提交 ===');
            console.log('选中的权限数量:', selectedPermissions.length);
            console.log('提交权限数据:', selectedPermissions);
            console.log('权限JSON:', permissionsJson);
            
            if (existingInput) {
                existingInput.value = permissionsJson;
                console.log('更新现有隐藏字段');
            } else {
                const permissionsInput = document.createElement('input');
                permissionsInput.type = 'hidden';
                permissionsInput.name = 'permissionsJson';
                permissionsInput.value = permissionsJson;
                form.appendChild(permissionsInput);
                console.log('创建新的隐藏字段');
            }
            
            // 验证数据
            if (selectedPermissions.length === 0) {
                console.warn('警告：没有选中任何权限');
            }
        });
    });

    // 初始化权限树
    if (document.getElementById('permissionTree')) {
        initPermissionTree();
    }
});

// 收集当前所有选中的权限（从DOM中读取）
function collectCurrentPermissions() {
    selectedPermissions = [];
    
    // 收集所有选中的菜单权限
    const menuCheckboxes = document.querySelectorAll('.menu-checkbox:checked');
    console.log('选中的菜单复选框数量:', menuCheckboxes.length);
    menuCheckboxes.forEach(checkbox => {
        const menuId = parseInt(checkbox.dataset.menuId);
        if (menuId && !isNaN(menuId)) {
            addPermission(menuId, null, 0);
        } else {
            console.warn('无效的菜单ID:', checkbox.dataset.menuId);
        }
    });
    
    // 收集所有选中的按钮权限
    const buttonCheckboxes = document.querySelectorAll('.button-checkbox:checked');
    console.log('选中的按钮复选框数量:', buttonCheckboxes.length);
    buttonCheckboxes.forEach(checkbox => {
        const menuId = parseInt(checkbox.dataset.menuId);
        const buttonId = parseInt(checkbox.dataset.buttonId);
        if (menuId && !isNaN(menuId) && buttonId && !isNaN(buttonId)) {
            addPermission(menuId, buttonId, 1);
        } else {
            console.warn('无效的按钮ID:', checkbox.dataset.menuId, checkbox.dataset.buttonId);
        }
    });
    
    console.log('收集到的权限总数:', selectedPermissions.length);
    console.log('收集到的权限详情:', selectedPermissions);
}

