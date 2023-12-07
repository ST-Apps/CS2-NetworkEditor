import React from 'react'

const $Section = ({ children, style, contentStyle, isList }) => {
    const classNames = "section_sop section_gUk" + (isList ? "list-section_BJP" : "statistics-menu_y86" );

    return <div className={classNames} style={{ width: 'auto', height: 'auto', overflowY: 'visible', ...style }}>
        <div className="content_flM content_owQ first_l25 last_ZNw" style={contentStyle}>
            {children}
        </div>
    </div>
}

export default $Section