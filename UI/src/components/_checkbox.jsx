import React from 'react'

const $CheckBox = ({ style, checked, onToggle }) => {
    const checked_class = checked ? 'checked' : 'unchecked';

    const handleClick = () => {
        onToggle(!checked)
    }

    const many = (...styles) => {
        return styles.join(' ')
    }


    return <div className={many('toggle_cca toggle_ATa', checked_class)} style={style} onClick={handleClick}>
        <div className={many('checkmark_NXV', checked_class)}></div>
    </div>

}

export default $CheckBox