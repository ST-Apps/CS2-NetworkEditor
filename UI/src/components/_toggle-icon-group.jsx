import React from 'react'
import $ToggleButton from './_toggle-button';

const $ToggleIconGroup = ({ react, style, options, isHorizontal, checked, onChecked }) => {
    let groupStyle = isHorizontal ? { flexDirection: 'row' } : { flexDirection: 'column' };

    const onToggle = (option) => {
        if (onChecked)
            onChecked(option);
    };

    const selectedIndex = options.indexOf(checked);

    return <div style={{ display: 'flex', width: '100%', ...groupStyle, ...style }}>
        {
            options.map((option, index) => {
                let optionStyle = {
                    flex: 1,
                    marginRight: index === options.length - 1 ? 0 : '2.5rem',
                    marginLeft: index === 0 ? 0 : '2.5rem'
                };
                let optionExtraStyle = isHorizontal ? {
                    alignItems: 'center',
                    justifyContent: 'center'
                } : {};
                return (
                    <$ToggleButton key={option}
                        icon={option}
                        checked={index === selectedIndex}
                        style={{ ...optionStyle, ...optionExtraStyle }}
                        onToggle={() => onToggle(option)}
                        iconOnly="true"
                    />
                );
            })
        }
    </div>
}

export default $ToggleIconGroup