// 全局弹窗管理器 - 用于在父窗口中显示弹窗（覆盖整个页面包括左侧菜单）

/**
 * 检查是否在iframe中
 */
function isInIframe() {
    try {
        return window.self !== window.top;
    } catch (e) {
        return true;
    }
}

/**
 * 获取目标窗口（如果在iframe中则返回父窗口，否则返回当前窗口）
 */
function getTargetWindow() {
    return isInIframe() ? window.parent : window;
}

/**
 * 获取目标文档
 */
function getTargetDocument() {
    return getTargetWindow().document;
}

/**
 * 在父窗口中显示弹窗
 * @param {string} modalHtml - 弹窗的HTML内容
 * @param {string} modalId - 弹窗的ID
 */
function showGlobalModal(modalHtml, modalId) {
    const targetDoc = getTargetDocument();
    const targetWindow = getTargetWindow();
    
    // 获取或创建全局弹窗容器
    let container = targetDoc.getElementById('globalModalContainer');
    if (!container) {
        container = targetDoc.createElement('div');
        container.id = 'globalModalContainer';
        targetDoc.body.appendChild(container);
    }
    
    // 设置弹窗HTML
    container.innerHTML = modalHtml;
    container.classList.add('show'); // 使用类来控制显示
    
    // 添加全局样式（如果还没有）
    if (!targetDoc.getElementById('globalModalStyles')) {
        const style = targetDoc.createElement('style');
        style.id = 'globalModalStyles';
        style.textContent = `
            #globalModalContainer {
                position: fixed !important;
                top: 0 !important;
                left: 0 !important;
                right: 0 !important;
                bottom: 0 !important;
                z-index: 10000 !important;
                background: rgba(0, 0, 0, 0.5) !important;
                display: none !important;
                align-items: center !important;
                justify-content: center !important;
            }
            #globalModalContainer.show {
                display: flex !important;
            }
            #globalModalContainer .modal-content {
                background: white;
                border-radius: 8px;
                width: 90%;
                max-width: 600px;
                max-height: 90vh;
                display: flex;
                flex-direction: column;
                box-shadow: 0 10px 40px rgba(0,0,0,0.2);
                position: relative;
                z-index: 10001;
            }
            #globalModalContainer .modal-header {
                padding: 1.25rem 1.5rem;
                border-bottom: 1px solid #e5e7eb;
                display: flex;
                justify-content: space-between;
                align-items: center;
            }
            #globalModalContainer .modal-header h3 {
                margin: 0;
                font-size: 1.25rem;
                font-weight: 600;
            }
            #globalModalContainer .modal-close {
                background: none;
                border: none;
                font-size: 1.5rem;
                color: #9ca3af;
                cursor: pointer;
                padding: 0;
                width: 2rem;
                height: 2rem;
                display: flex;
                align-items: center;
                justify-content: center;
                border-radius: 4px;
                transition: all 0.2s;
            }
            #globalModalContainer .modal-close:hover {
                background-color: #f3f4f6;
                color: #374151;
            }
            #globalModalContainer .modal-body {
                flex: 1;
                overflow-y: auto;
                padding: 1.5rem;
            }
            #globalModalContainer .modal-footer {
                padding: 1rem 1.5rem;
                border-top: 1px solid #e5e7eb;
                display: flex;
                justify-content: flex-end;
                gap: 0.75rem;
            }
        `;
        targetDoc.head.appendChild(style);
    }
}

/**
 * 关闭全局弹窗
 */
function closeGlobalModal() {
    const targetDoc = getTargetDocument();
    const container = targetDoc.getElementById('globalModalContainer');
    if (container) {
        container.classList.remove('show'); // 移除show类来隐藏
        container.innerHTML = '';
    }
}

/**
 * 获取弹窗HTML模板
 */
function getModalHtml(title, formId, formContent, onSaveFunction) {
    return `
        <div class="modal-content">
            <div class="modal-header">
                <h3>${title}</h3>
                <button class="modal-close" onclick="closeGlobalModal()">×</button>
            </div>
            <div class="modal-body">
                <form id="${formId}">
                    ${formContent}
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn-save" onclick="${onSaveFunction}">保存</button>
                <button class="btn-cancel" onclick="closeGlobalModal()">取消</button>
            </div>
        </div>
    `;
}

