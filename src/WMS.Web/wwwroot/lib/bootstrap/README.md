# Bootstrap 6 本地文件说明

## 下载 Bootstrap 6

请从以下地址下载 Bootstrap 6 的 CSS 和 JS 文件，并放置到对应目录：

### 下载地址
- Bootstrap 6 CSS: https://cdn.jsdelivr.net/npm/bootstrap@6.0.0/dist/css/bootstrap.min.css
- Bootstrap 6 JS: https://cdn.jsdelivr.net/npm/bootstrap@6.0.0/dist/js/bootstrap.bundle.min.js

### 目录结构
```
wwwroot/
  lib/
    bootstrap/
      css/
        bootstrap.min.css
      js/
        bootstrap.bundle.min.js
```

### 下载方法

#### 方法1：使用浏览器下载
1. 访问上述链接
2. 右键保存文件到对应目录

#### 方法2：使用命令行（PowerShell）
```powershell
# 创建目录
New-Item -ItemType Directory -Force -Path "wwwroot\lib\bootstrap\css"
New-Item -ItemType Directory -Force -Path "wwwroot\lib\bootstrap\js"

# 下载文件
Invoke-WebRequest -Uri "https://cdn.jsdelivr.net/npm/bootstrap@6.0.0/dist/css/bootstrap.min.css" -OutFile "wwwroot\lib\bootstrap\css\bootstrap.min.css"
Invoke-WebRequest -Uri "https://cdn.jsdelivr.net/npm/bootstrap@6.0.0/dist/js/bootstrap.bundle.min.js" -OutFile "wwwroot\lib\bootstrap\js\bootstrap.bundle.min.js"
```

#### 方法3：使用 npm（如果已安装 Node.js）
```bash
npm install bootstrap@6.0.0
# 然后从 node_modules/bootstrap/dist/ 复制文件到 wwwroot/lib/bootstrap/
```

### 注意事项
- 确保文件名为 `bootstrap.min.css` 和 `bootstrap.bundle.min.js`
- 确保文件路径与 `_Layout.cshtml` 中的引用路径一致
- 如果使用其他版本，请相应更新版本号

