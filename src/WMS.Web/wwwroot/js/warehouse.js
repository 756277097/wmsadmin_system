// 仓库管理页面交互逻辑

let selectedNode = null;
let selectedType = null;

// 切换节点展开/折叠
function toggleNode(element) {
    const node = element.closest('.tree-node');
    const children = node.querySelector('.tree-node-children');
    
    if (children) {
        const isExpanded = children.style.display !== 'none';
        children.style.display = isExpanded ? 'none' : 'block';
        node.classList.toggle('expanded', !isExpanded);
    }
}

// 选择节点
function selectNode(element, type, id, parentId) {
    // 移除之前的选中状态
    document.querySelectorAll('.tree-node-header').forEach(header => {
        header.classList.remove('active');
    });
    
    // 添加选中状态
    element.classList.add('active');
    
    selectedNode = { type, id, parentId };
    selectedType = type;
    
    // 加载详情
    loadDetail(type, id);
}

// 加载详情
async function loadDetail(type, id) {
    const detailContent = document.getElementById('detailContent');
    detailContent.innerHTML = '<div style="text-align: center; padding: 2rem;"><div class="spinner-border" role="status"><span class="visually-hidden">加载中...</span></div></div>';
    
    try {
        let url = '';
        if (type === 'warehouse') {
            url = `/Warehouse/GetWarehouse?id=${id}`;
        } else if (type === 'zone') {
            url = `/Warehouse/GetZone?id=${id}`;
        } else if (type === 'location') {
            url = `/Warehouse/GetLocation?id=${id}`;
        }
        
        const response = await fetch(url);
        if (!response.ok) throw new Error('加载失败');
        
        const data = await response.json();
        renderDetail(type, data);
    } catch (error) {
        detailContent.innerHTML = `<div class="alert alert-danger">加载失败：${error.message}</div>`;
    }
}

// 渲染详情
function renderDetail(type, data) {
    const detailContent = document.getElementById('detailContent');
    
    if (type === 'warehouse') {
        detailContent.innerHTML = `
            <div class="card-modern">
                <div class="card-body-modern">
                    <div class="detail-header">
                        <h3>仓库详情</h3>
                        <div class="detail-actions">
                            <button class="btn-action btn-edit" onclick="showEditWarehouse(${data.id})" data-permission="Warehouse:Edit" data-permission-action="hide">编辑</button>
                            <button class="btn-action btn-delete" onclick="deleteWarehouse(${data.id})" data-permission="Warehouse:Delete" data-permission-action="hide">删除</button>
                        </div>
                    </div>
                    <div class="detail-info">
                        <div class="info-row">
                            <span class="info-label">编码：</span>
                            <span class="info-value">${data.code}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">名称：</span>
                            <span class="info-value">${data.name}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">地址：</span>
                            <span class="info-value">${data.address || '-'}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">联系人：</span>
                            <span class="info-value">${data.contactPerson || '-'}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">联系电话：</span>
                            <span class="info-value">${data.contactPhone || '-'}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">状态：</span>
                            <span class="info-value">
                                <span class="badge-status ${data.isEnabled ? 'badge-success' : 'badge-danger'}">
                                    <span class="badge-dot"></span>
                                    ${data.isEnabled ? '启用' : '禁用'}
                                </span>
                            </span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">备注：</span>
                            <span class="info-value">${data.remarks || '-'}</span>
                        </div>
                    </div>
                </div>
            </div>
        `;
    } else if (type === 'zone') {
        const zoneTypeNames = ['原料区', '成品区', '暂存区', '退料区', '不良品区'];
        detailContent.innerHTML = `
            <div class="card-modern">
                <div class="card-body-modern">
                    <div class="detail-header">
                        <h3>库区详情</h3>
                        <div class="detail-actions">
                            <button class="btn-action btn-edit" onclick="showEditZone(${data.id}, ${data.warehouseId})" data-permission="Warehouse:Edit" data-permission-action="hide">编辑</button>
                            <button class="btn-action btn-delete" onclick="deleteZone(${data.id})" data-permission="Warehouse:Delete" data-permission-action="hide">删除</button>
                        </div>
                    </div>
                    <div class="detail-info">
                        <div class="info-row">
                            <span class="info-label">编码：</span>
                            <span class="info-value">${data.code}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">名称：</span>
                            <span class="info-value">${data.name}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">类型：</span>
                            <span class="info-value">
                                <span class="badge-status badge-primary">${zoneTypeNames[data.zoneType] || '未知'}</span>
                            </span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">状态：</span>
                            <span class="info-value">
                                <span class="badge-status ${data.isEnabled ? 'badge-success' : 'badge-danger'}">
                                    <span class="badge-dot"></span>
                                    ${data.isEnabled ? '启用' : '禁用'}
                                </span>
                            </span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">备注：</span>
                            <span class="info-value">${data.remarks || '-'}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">库位数量：</span>
                            <span class="info-value">${data.locations ? data.locations.length : 0}</span>
                        </div>
                    </div>
                </div>
            </div>
        `;
    } else if (type === 'location') {
        detailContent.innerHTML = `
            <div class="card-modern">
                <div class="card-body-modern">
                    <div class="detail-header">
                        <h3>库位详情</h3>
                        <div class="detail-actions">
                            <button class="btn-action btn-edit" onclick="showEditLocation(${data.id}, ${data.zoneId})" data-permission="Warehouse:Edit" data-permission-action="hide">编辑</button>
                            <button class="btn-action btn-delete" onclick="deleteLocation(${data.id})" data-permission="Warehouse:Delete" data-permission-action="hide">删除</button>
                        </div>
                    </div>
                    <div class="detail-info">
                        <div class="info-row">
                            <span class="info-label">编码：</span>
                            <span class="info-value">${data.code}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">名称：</span>
                            <span class="info-value">${data.name}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">属性：</span>
                            <span class="info-value">
                                <span class="badge-status badge-secondary">${data.locationType === 0 ? '固定' : '随机'}</span>
                            </span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">体积限制：</span>
                            <span class="info-value">${data.volumeLimit ? data.volumeLimit + ' 立方米' : '-'}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">重量限制：</span>
                            <span class="info-value">${data.weightLimit ? data.weightLimit + ' 千克' : '-'}</span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">状态：</span>
                            <span class="info-value">
                                <span class="badge-status ${data.isEnabled ? 'badge-success' : 'badge-danger'}">
                                    <span class="badge-dot"></span>
                                    ${data.isEnabled ? '启用' : '禁用'}
                                </span>
                            </span>
                        </div>
                        <div class="info-row">
                            <span class="info-label">备注：</span>
                            <span class="info-value">${data.remarks || '-'}</span>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }
    
    // 重新初始化权限控制
    if (typeof controlButtons === 'function') {
        controlButtons();
    }
}

// 获取弹窗HTML内容
function getWarehouseModalHtml(title, data = null) {
    const id = data ? data.id : '';
    const code = data ? data.code : '';
    const name = data ? data.name : '';
    const address = data ? (data.address || '') : '';
    const contactPerson = data ? (data.contactPerson || '') : '';
    const contactPhone = data ? (data.contactPhone || '') : '';
    const remarks = data ? (data.remarks || '') : '';
    const isEnabled = data ? data.isEnabled : true;
    
    return `
        <div class="modal-content">
            <div class="modal-header">
                <h3>${title}</h3>
                <button class="modal-close" onclick="closeGlobalModal()">×</button>
            </div>
            <div class="modal-body">
                <form id="warehouseForm">
                    <input type="hidden" id="warehouseId" value="${id}" />
                    <div class="form-group-modern">
                        <label class="form-label-modern">仓库编码 <span class="text-danger">*</span></label>
                        <input type="text" id="warehouseCode" class="form-control-modern" value="${code}" required />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">仓库名称 <span class="text-danger">*</span></label>
                        <input type="text" id="warehouseName" class="form-control-modern" value="${name}" required />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">仓库地址</label>
                        <input type="text" id="warehouseAddress" class="form-control-modern" value="${address}" />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">联系人</label>
                        <input type="text" id="warehouseContactPerson" class="form-control-modern" value="${contactPerson}" />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">联系电话</label>
                        <input type="text" id="warehouseContactPhone" class="form-control-modern" value="${contactPhone}" />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">备注</label>
                        <textarea id="warehouseRemarks" class="form-control-modern" rows="3">${remarks}</textarea>
                    </div>
                    <div class="form-group-modern">
                        <div class="form-check-modern">
                            <input type="checkbox" id="warehouseIsEnabled" class="form-check-input-modern" ${isEnabled ? 'checked' : ''} />
                            <label class="form-check-label-modern" for="warehouseIsEnabled">启用</label>
                        </div>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn-save" onclick="saveWarehouseFromGlobal()">保存</button>
                <button class="btn-cancel" onclick="closeGlobalModal()">取消</button>
            </div>
        </div>
    `;
}

// 仓库操作
function showCreateWarehouse() {
    const targetWindow = isInIframe() ? window.parent : window;
    const modalHtml = getWarehouseModalHtml('新增仓库');
    showGlobalModal(modalHtml, 'warehouseModal');
}

function showEditWarehouse(id) {
    fetch(`/Warehouse/GetWarehouse?id=${id}`)
        .then(res => res.json())
        .then(data => {
            const targetWindow = isInIframe() ? window.parent : window;
            const modalHtml = getWarehouseModalHtml('编辑仓库', data);
            showGlobalModal(modalHtml, 'warehouseModal');
        });
}

function closeWarehouseModal() {
    closeGlobalModal();
}

// 从全局弹窗中保存仓库
async function saveWarehouseFromGlobal() {
    const targetDoc = isInIframe() ? window.parent.document : document;
    const id = targetDoc.getElementById('warehouseId').value;
    const data = {
        id: id ? parseInt(id) : 0,
        code: targetDoc.getElementById('warehouseCode').value,
        name: targetDoc.getElementById('warehouseName').value,
        address: targetDoc.getElementById('warehouseAddress').value,
        contactPerson: targetDoc.getElementById('warehouseContactPerson').value,
        contactPhone: targetDoc.getElementById('warehouseContactPhone').value,
        remarks: targetDoc.getElementById('warehouseRemarks').value,
        isEnabled: targetDoc.getElementById('warehouseIsEnabled').checked
    };
    
    const url = id ? '/Warehouse/UpdateWarehouse' : '/Warehouse/CreateWarehouse';
    const response = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    
    const result = await response.json();
    if (result.success) {
        alert('保存成功');
        closeGlobalModal();
        location.reload();
    } else {
        alert('保存失败：' + result.message);
    }
}

// 保留原函数作为备用（如果不在iframe中）
async function saveWarehouse() {
    saveWarehouseFromGlobal();
}

async function deleteWarehouse(id) {
    if (!confirm('确定要删除该仓库吗？删除后库区和库位也会被删除。')) return;
    
    const response = await fetch('/Warehouse/DeleteWarehouse', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(id)
    });
    
    const result = await response.json();
    if (result.success) {
        alert('删除成功');
        location.reload();
    } else {
        alert('删除失败');
    }
}

// 获取库区弹窗HTML
function getZoneModalHtml(title, warehouseId, data = null) {
    const id = data ? data.id : '';
    const code = data ? data.code : '';
    const name = data ? data.name : '';
    const zoneType = data ? data.zoneType : 0;
    const remarks = data ? (data.remarks || '') : '';
    const isEnabled = data ? data.isEnabled : true;
    
    return `
        <div class="modal-content">
            <div class="modal-header">
                <h3>${title}</h3>
                <button class="modal-close" onclick="closeGlobalModal()">×</button>
            </div>
            <div class="modal-body">
                <form id="zoneForm">
                    <input type="hidden" id="zoneId" value="${id}" />
                    <input type="hidden" id="zoneWarehouseId" value="${warehouseId}" />
                    <div class="form-group-modern">
                        <label class="form-label-modern">库区编码 <span class="text-danger">*</span></label>
                        <input type="text" id="zoneCode" class="form-control-modern" value="${code}" required />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">库区名称 <span class="text-danger">*</span></label>
                        <input type="text" id="zoneName" class="form-control-modern" value="${name}" required />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">库区类型 <span class="text-danger">*</span></label>
                        <select id="zoneType" class="form-control-modern" required>
                            <option value="0" ${zoneType === 0 ? 'selected' : ''}>原料区</option>
                            <option value="1" ${zoneType === 1 ? 'selected' : ''}>成品区</option>
                            <option value="2" ${zoneType === 2 ? 'selected' : ''}>暂存区</option>
                            <option value="3" ${zoneType === 3 ? 'selected' : ''}>退料区</option>
                            <option value="4" ${zoneType === 4 ? 'selected' : ''}>不良品区</option>
                        </select>
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">备注</label>
                        <textarea id="zoneRemarks" class="form-control-modern" rows="3">${remarks}</textarea>
                    </div>
                    <div class="form-group-modern">
                        <div class="form-check-modern">
                            <input type="checkbox" id="zoneIsEnabled" class="form-check-input-modern" ${isEnabled ? 'checked' : ''} />
                            <label class="form-check-label-modern" for="zoneIsEnabled">启用</label>
                        </div>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn-save" onclick="saveZoneFromGlobal()">保存</button>
                <button class="btn-cancel" onclick="closeGlobalModal()">取消</button>
            </div>
        </div>
    `;
}

// 库区操作
function showCreateZone(warehouseId) {
    const modalHtml = getZoneModalHtml('新增库区', warehouseId);
    showGlobalModal(modalHtml, 'zoneModal');
}

function showEditZone(id, warehouseId) {
    fetch(`/Warehouse/GetZone?id=${id}`)
        .then(res => res.json())
        .then(data => {
            const modalHtml = getZoneModalHtml('编辑库区', data.warehouseId, data);
            showGlobalModal(modalHtml, 'zoneModal');
        });
}

function closeZoneModal() {
    closeGlobalModal();
}

// 从全局弹窗中保存库区
async function saveZoneFromGlobal() {
    const targetDoc = isInIframe() ? window.parent.document : document;
    const id = targetDoc.getElementById('zoneId').value;
    const data = {
        id: id ? parseInt(id) : 0,
        warehouseId: parseInt(targetDoc.getElementById('zoneWarehouseId').value),
        code: targetDoc.getElementById('zoneCode').value,
        name: targetDoc.getElementById('zoneName').value,
        zoneType: parseInt(targetDoc.getElementById('zoneType').value),
        remarks: targetDoc.getElementById('zoneRemarks').value,
        isEnabled: targetDoc.getElementById('zoneIsEnabled').checked
    };
    
    const url = id ? '/Warehouse/UpdateZone' : '/Warehouse/CreateZone';
    const response = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    
    const result = await response.json();
    if (result.success) {
        alert('保存成功');
        closeGlobalModal();
        location.reload();
    } else {
        alert('保存失败：' + result.message);
    }
}

// 保留原函数作为备用
async function saveZone() {
    saveZoneFromGlobal();
}

async function deleteZone(id) {
    if (!confirm('确定要删除该库区吗？删除后库位也会被删除。')) return;
    
    const response = await fetch('/Warehouse/DeleteZone', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(id)
    });
    
    const result = await response.json();
    if (result.success) {
        alert('删除成功');
        location.reload();
    } else {
        alert('删除失败');
    }
}

// 获取库位弹窗HTML
function getLocationModalHtml(title, zoneId, data = null) {
    const id = data ? data.id : '';
    const code = data ? data.code : '';
    const name = data ? data.name : '';
    const locationType = data ? data.locationType : 0;
    const volumeLimit = data ? (data.volumeLimit || '') : '';
    const weightLimit = data ? (data.weightLimit || '') : '';
    const remarks = data ? (data.remarks || '') : '';
    const isEnabled = data ? data.isEnabled : true;
    
    return `
        <div class="modal-content">
            <div class="modal-header">
                <h3>${title}</h3>
                <button class="modal-close" onclick="closeGlobalModal()">×</button>
            </div>
            <div class="modal-body">
                <form id="locationForm">
                    <input type="hidden" id="locationId" value="${id}" />
                    <input type="hidden" id="locationZoneId" value="${zoneId}" />
                    <div class="form-group-modern">
                        <label class="form-label-modern">库位编码 <span class="text-danger">*</span></label>
                        <input type="text" id="locationCode" class="form-control-modern" value="${code}" required />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">库位名称 <span class="text-danger">*</span></label>
                        <input type="text" id="locationName" class="form-control-modern" value="${name}" required />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">库位属性 <span class="text-danger">*</span></label>
                        <select id="locationType" class="form-control-modern" required>
                            <option value="0" ${locationType === 0 ? 'selected' : ''}>固定</option>
                            <option value="1" ${locationType === 1 ? 'selected' : ''}>随机</option>
                        </select>
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">体积限制（立方米）</label>
                        <input type="number" id="locationVolumeLimit" class="form-control-modern" step="0.01" value="${volumeLimit}" />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">重量限制（千克）</label>
                        <input type="number" id="locationWeightLimit" class="form-control-modern" step="0.01" value="${weightLimit}" />
                    </div>
                    <div class="form-group-modern">
                        <label class="form-label-modern">备注</label>
                        <textarea id="locationRemarks" class="form-control-modern" rows="3">${remarks}</textarea>
                    </div>
                    <div class="form-group-modern">
                        <div class="form-check-modern">
                            <input type="checkbox" id="locationIsEnabled" class="form-check-input-modern" ${isEnabled ? 'checked' : ''} />
                            <label class="form-check-label-modern" for="locationIsEnabled">启用</label>
                        </div>
                    </div>
                </form>
            </div>
            <div class="modal-footer">
                <button class="btn-save" onclick="saveLocationFromGlobal()">保存</button>
                <button class="btn-cancel" onclick="closeGlobalModal()">取消</button>
            </div>
        </div>
    `;
}

// 库位操作
function showCreateLocation(zoneId) {
    const modalHtml = getLocationModalHtml('新增库位', zoneId);
    showGlobalModal(modalHtml, 'locationModal');
}

function showEditLocation(id, zoneId) {
    fetch(`/Warehouse/GetLocation?id=${id}`)
        .then(res => res.json())
        .then(data => {
            const modalHtml = getLocationModalHtml('编辑库位', data.zoneId, data);
            showGlobalModal(modalHtml, 'locationModal');
        });
}

function closeLocationModal() {
    closeGlobalModal();
}

// 从全局弹窗中保存库位
async function saveLocationFromGlobal() {
    const targetDoc = isInIframe() ? window.parent.document : document;
    const id = targetDoc.getElementById('locationId').value;
    const data = {
        id: id ? parseInt(id) : 0,
        zoneId: parseInt(targetDoc.getElementById('locationZoneId').value),
        code: targetDoc.getElementById('locationCode').value,
        name: targetDoc.getElementById('locationName').value,
        locationType: parseInt(targetDoc.getElementById('locationType').value),
        volumeLimit: targetDoc.getElementById('locationVolumeLimit').value ? parseFloat(targetDoc.getElementById('locationVolumeLimit').value) : null,
        weightLimit: targetDoc.getElementById('locationWeightLimit').value ? parseFloat(targetDoc.getElementById('locationWeightLimit').value) : null,
        remarks: targetDoc.getElementById('locationRemarks').value,
        isEnabled: targetDoc.getElementById('locationIsEnabled').checked
    };
    
    const url = id ? '/Warehouse/UpdateLocation' : '/Warehouse/CreateLocation';
    const response = await fetch(url, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data)
    });
    
    const result = await response.json();
    if (result.success) {
        alert('保存成功');
        closeGlobalModal();
        location.reload();
    } else {
        alert('保存失败：' + result.message);
    }
}

// 保留原函数作为备用
async function saveLocation() {
    saveLocationFromGlobal();
}

async function deleteLocation(id) {
    if (!confirm('确定要删除该库位吗？')) return;
    
    const response = await fetch('/Warehouse/DeleteLocation', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(id)
    });
    
    const result = await response.json();
    if (result.success) {
        alert('删除成功');
        location.reload();
    } else {
        alert('删除失败');
    }
}

// 点击仓库节点时选择仓库
document.addEventListener('DOMContentLoaded', function() {
    document.querySelectorAll('.warehouse-node .tree-node-header').forEach(header => {
        header.addEventListener('click', function(e) {
            if (e.target.classList.contains('tree-node-toggle')) return;
            const node = this.closest('.warehouse-node');
            const id = parseInt(node.dataset.id);
            selectNode(this, 'warehouse', id, null);
        });
    });
    
    document.querySelectorAll('.zone-node .tree-node-header').forEach(header => {
        header.addEventListener('click', function(e) {
            if (e.target.classList.contains('tree-node-toggle')) return;
            const node = this.closest('.zone-node');
            const id = parseInt(node.dataset.id);
            const warehouseId = parseInt(node.dataset.warehouseId);
            selectNode(this, 'zone', id, warehouseId);
        });
    });
});

