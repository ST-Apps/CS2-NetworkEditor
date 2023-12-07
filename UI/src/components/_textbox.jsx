import React from 'react'
import ReactDOM from 'react-dom';

const $TextBox = ({ react, style, value, children }) => {
    const [internalValue, setInternalValue] = react.useState('Select...');
    const pickerRef = react.useRef(null); // Ref to attach to the select field


    return (<div style={{ width: '100%' }}>
        <div ref={pickerRef} className="dropdown-toggle_V9z dropdown-toggle_prl value-field_yJi value_PW_ dropdown_pJu item-states_QjV" style={{ padding: '10rem', height: 'auto', ...style }}>
            <div className="label_l_4">{internalValue}</div>
        </div>
    </div>);
}

export default $TextBox