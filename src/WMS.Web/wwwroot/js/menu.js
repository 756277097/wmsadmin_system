// 菜单树渲染和交互

let userMenus = [];

// 初始化菜单
function initMenu() {
    // 从页面中获取菜单数据（由后端注入）
    const menuDataElement = document.getElementById('menuData');
    if (menuDataElement) {
        try {
            const menuDataText = menuDataElement.textContent;
            userMenus = JSON.parse(menuDataText) || [];
            console.log('菜单数据加载成功，共', userMenus.length, '个顶级菜单');
            if (userMenus.length > 0) {
                console.log('菜单列表:', userMenus.map(m => m.name));
                console.log('菜单数据示例:', userMenus[0]);
            } else {
                console.warn('菜单数据为空，请检查用户权限配置');
            }
        } catch (e) {
            console.error('解析菜单数据失败:', e);
            console.error('菜单数据内容:', menuDataElement.textContent);
            userMenus = [];
        }
    } else {
        // 如果没有注入数据，尝试从API获取
        console.log('未找到菜单数据元素，尝试从API获取...');
        loadMenusFromApi();
        return; // loadMenusFromApi 会调用 renderMenuTree
    }

    renderMenuTree();
}

// 从API加载菜单
async function loadMenusFromApi() {
    try {
        const response = await fetch('/Auth/GetUserMenus');
        if (response.ok) {
            userMenus = await response.json();
            renderMenuTree();
        }
    } catch (error) {
        console.error('加载菜单失败:', error);
    }
}

// 渲染菜单树
function renderMenuTree() {
    const menuTree = document.getElementById('menuTree');
    if (!menuTree) {
        console.warn('未找到菜单树容器 #menuTree');
        return;
    }

    menuTree.innerHTML = '';

    if (userMenus.length === 0) {
        menuTree.innerHTML = '<li class="nav-item"><span class="text-muted ps-3">暂无可用菜单，请联系管理员分配权限</span></li>';
        console.warn('用户没有可用菜单，请检查角色权限配置');
        return;
    }

    // 渲染顶级菜单
    userMenus.forEach(menu => {
        const menuItem = createMenuItem(menu);
        menuTree.appendChild(menuItem);
    });
    
    console.log('菜单树渲染完成');
}

// 创建菜单项
function createMenuItem(menu) {
    const li = document.createElement('li');
    li.className = 'nav-item';

    // 使用 camelCase 属性（已配置 JSON 序列化为 camelCase）
    const menuId = menu.id;
    const menuName = menu.name;
    const menuType = menu.menuType;
    const menuPath = menu.path || '';
    const menuIcon = menu.icon || '📁';
    const menuChildren = menu.children || [];

    const hasChildren = menuChildren && menuChildren.length > 0;

    let menuHtml = `
        <a class="nav-link menu-item ${hasChildren ? 'has-children' : ''}" 
           data-menu-id="${menuId}" 
           data-menu-type="${menuType}"
           data-menu-path="${menuPath}"
           href="javascript:void(0)">
            <span class="menu-icon">${menuIcon}</span>
            <span class="menu-text">${menuName}</span>
    `;

    if (hasChildren) {
        menuHtml += '<span class="menu-toggle ms-auto">▼</span>';
    }

    menuHtml += '</a>';

    li.innerHTML = menuHtml;

    // 添加子菜单
    if (hasChildren) {
        const childrenUl = document.createElement('ul');
        childrenUl.className = 'nav flex-column menu-children';

        menuChildren.forEach(child => {
            const childItem = createMenuItem(child);
            childrenUl.appendChild(childItem);
            
            // 子菜单项点击事件
            const childLink = childItem.querySelector('.menu-item');
            if (childLink && !child.menuChildren || child.menuChildren.length === 0) {
                childLink.addEventListener('click', function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (child.path) {
                        loadMenuPage(child);
                    }
                });
            }
        });

        li.appendChild(childrenUl);

        // 点击展开/收起（平滑动画）
        const menuLink = li.querySelector('.menu-item');
        menuLink.addEventListener('click', function(e) {
            e.preventDefault();
            e.stopPropagation();
            
            const isExpanded = menuLink.classList.contains('expanded');
            
            if (isExpanded) {
                // 收起
                menuLink.classList.remove('expanded');
                childrenUl.classList.remove('show');
            } else {
                // 展开
                menuLink.classList.add('expanded');
                childrenUl.classList.add('show');
            }
        });
    } else {
        // 点击菜单项加载页面
        const menuLink = li.querySelector('.menu-item');
        menuLink.addEventListener('click', function(e) {
            e.preventDefault();
            // 如果菜单有路径，加载页面
            if (menuPath) {
                loadMenuPage(menu);
            }
        });
    }

    return li;
}

// 加载菜单页面
function loadMenuPage(menu) {
    const menuId = menu.id;
    const menuType = menu.menuType;
    const menuPath = menu.path || '';

    // 移除其他菜单的active状态
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
    });

    // 添加当前菜单的active状态
    const currentLink = document.querySelector(`[data-menu-id="${menuId}"]`);
    if (currentLink) {
        currentLink.classList.add('active');
    }

    // 根据菜单类型加载页面
    if (menuType === 0) {
        // 内部页面 - 使用iframe加载
        if (menuPath) {
            const contentArea = document.getElementById('contentArea');
            if (contentArea) {
                // 隐藏初始内容
                const initialContent = document.getElementById('initialContent');
                if (initialContent) {
                    initialContent.style.display = 'none';
                }
                
                // 创建或更新iframe
                let iframe = document.getElementById('mainIframe');
                if (!iframe) {
                    iframe = document.createElement('iframe');
                    iframe.id = 'mainIframe';
                    iframe.name = 'mainIframe';
                    iframe.frameBorder = '0';
                    iframe.style.width = '100%';
                    iframe.style.height = '100%';
                    iframe.style.border = 'none';
                    iframe.style.minHeight = 'calc(100vh - 200px)';
                    iframe.style.display = 'block';
                    contentArea.appendChild(iframe);
                } else {
                    iframe.style.display = 'block';
                }
                
                // 处理路径：确保路径格式正确
                let finalPath = menuPath;
                
                // 如果路径是 /User、/Role 等，需要转换为 /User/Index、/Role/Index
                if (menuPath.startsWith('/') && menuPath !== '#' && menuPath !== '/') {
                    const pathParts = menuPath.split('/').filter(p => p);
                    // 如果只有控制器名（如 /User），添加 /Index
                    if (pathParts.length === 1) {
                        finalPath = menuPath + '/Index';
                    }
                    // 如果路径已经是完整路径（如 /User/Index），保持不变
                }
                
                // 加载页面
                console.log('加载菜单页面 - 原始路径:', menuPath, '最终路径:', finalPath);
                iframe.src = finalPath;
            }
        }
    } else {
        // 外部链接 - 在新窗口打开
        if (menuPath) {
            window.open(menuPath, '_blank');
        }
    }
}

// 刷新菜单（从服务器重新获取）
async function refreshMenu() {
    try {
        const response = await fetch('/Menu/GetTree');
        const menus = await response.json();
        userMenus = menus;
        renderMenuTree();
    } catch (error) {
        console.error('刷新菜单失败:', error);
    }
}

