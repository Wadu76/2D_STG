# ESC暂停菜单设置指南

## 1. 创建PauseMenuManager游戏对象

1. 在Hierarchy中右键点击空白区域
2. 选择 `Create Empty` 创建一个空游戏对象
3. 将其命名为 "PauseMenuManager"

## 2. 添加PauseMenuManager脚本

1. 选中PauseMenuManager对象
2. 在Inspector中，点击 `Add Component`
3. 搜索并添加 `PauseMenuManager` 脚本

## 3. 创建暂停面板UI

1. 在Hierarchy中右键点击PauseMenuManager
2. 选择 `UI` -> `Canvas` 创建画布
3. 在Canvas下创建 `Panel` 作为暂停面板背景
4. 设置Panel的宽度和高度（建议800x600）
5. 设置Panel的颜色为半透明黑色（如RGBA: 0, 0, 0, 150）

## 4. 创建文本和按钮

### 创建标题文本
1. 在Panel下创建 `Text - TextMeshPro`
2. 设置文本为 "PAUSED"
3. 调整字体大小和位置

### 创建玩家属性文本
在Panel下创建以下文本（用于显示玩家属性）：
1. **HealthText** - 显示当前血量
2. **SpeedText** - 显示移速加成
3. **DamageText** - 显示伤害加成
4. **FireRateText** - 显示射速加成
5. **IceChanceText** - 显示冰冻概率
6. **ShotgunText** - 显示散弹状态

### 创建继续游戏按钮
1. 在Panel下创建 `Button - TextMeshPro`
2. 设置文本为 "Resume" 或 "继续"
3. 添加点击事件：
   - 在OnClick事件中，拖拽PauseMenuManager对象
   - 选择函数：`PauseMenuManager.ResumeGame`

## 5. 配置PauseMenuManager

在Inspector中配置PauseMenuManager脚本：

### 引用设置
- **Pause Panel**: 拖拽创建的Panel对象到这里
- **Health Text**: 拖拽HealthText文本对象
- **Speed Text**: 拖拽SpeedText文本对象
- **Damage Text**: 拖拽DamageText文本对象
- **Fire Rate Text**: 拖拽FireRateText文本对象
- **Ice Chance Text**: 拖拽IceChanceText文本对象
- **Shotgun Text**: 拖拽ShotgunText文本对象

## 6. 调整UI布局建议

```
+----------------------------------+
|                                  |
|            PAUSED                |
|                                  |
|   Health: 100                    |
|   Speed: x1.00                   |
|   Damage: x1.00                  |
|   Fire Rate: x1.00               |
|   Ice Chance: 0%                  |
|   Shotgun: OFF                   |
|                                  |
|          [Resume]                 |
|                                  |
+----------------------------------+
```

### 文本位置调整
- 所有文本水平居中，垂直均匀分布
- 字体大小建议28-36
- 行间距适当调整

### 按钮位置
- 放在面板底部
- 按钮宽度建议200，高度50

## 7. 测试ESC暂停功能

1. 运行游戏
2. 按ESC键应该能打开/关闭暂停菜单
3. 暂停时游戏应该停止
4. 点击Resume按钮应该恢复游戏
5. 检查所有属性文本是否正确显示

## 8. 可能遇到的问题

### 问题1：按ESC没有反应
- 检查PauseMenuManager是否正确添加到了游戏对象
- 检查pausePanel引用是否正确赋值

### 问题2：属性显示为"--"
- 检查各个Manager的Instance是否为null
- 确保PlayerHealth、PlayerController、PlayerShooter都已初始化

### 问题3：UI不显示
- 检查Canvas是否正确创建
- 检查Panel的Active状态
- 检查Canvas的Render Mode设置

## 9. 自定义设置

### 修改冰冻效果颜色
在BulletController.cs中修改：
```csharp
spriteRenderer.color = new Color(0.5f, 0.8f, 1f, 0.8f); // 冰蓝色
```
可以调整RGB值来改变冰冻时的颜色。

### 修改冰冻持续时间
在Inspector中修改BulletController的 `Freeze Duration` 参数，默认1秒。

### 修改UI样式
可以自定义文本颜色、字体、背景样式来匹配游戏风格。

---

完成以上步骤后，你的ESC暂停菜单就可以正常工作了！