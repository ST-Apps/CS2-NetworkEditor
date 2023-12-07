import React from 'react'

const $ToggleButton = ({ label, icon, children, style, labelStyle, checked, onToggle, iconOnly }) => {
    const baseClassNames = "item_JFN button_ECf item_It6 item-mouse-states_Fmi item-selected_tAM item-focused_FuT";
    const checkedClassNames = checked ? " selected" : "";
    const classNames = baseClassNames + checkedClassNames;
    const checkedTextStyle = checked ? { color: 'var(--normalTextColor)' } : {};
    const iconMarkup = icon ? <img className="icon_HoD icon_soN icon_Iwk" src={icon} /> : "";

    const body = iconOnly ? "" : <div className="title_sB9" style={{ ...checkedTextStyle, ...labelStyle }}>{label}</div>;

    return <div className={classNames} style={style} onClick={onToggle}>
        {iconMarkup}
        {body}
        {children}
    </div>
}

export default $ToggleButton