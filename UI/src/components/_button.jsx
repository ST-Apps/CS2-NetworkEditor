import React from 'react'

const $Button = ({ children, style, onClick, isBlack }) => {
    const className = isBlack ? "button_KVN" : "button_WWa";
    const inlineStyle = isBlack ? { color: 'var(--textColor)' } : {};
    return <div>
        <button className={className} style={{ width: '100%', height: 'auto', padding: '10rem', ...inlineStyle, ...style }} onClick={onClick}>
            {children}
        </button>
    </div>
}

export default $Button