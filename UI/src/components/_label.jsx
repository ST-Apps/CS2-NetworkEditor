import React from 'react'

const $Label = ({ children, style, isBold }) => {
    var markup = isBold ? <b>{children}</b> : children;

    return <div className="label_VSW label_T__" styles={style}>
        {markup}
    </div>
}

export default $Label