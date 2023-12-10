import React from 'react'
import ReactDOM from 'react-dom';
import { $Field } from 'hookui-framework'

const $Select = ({ react, style, onToggle, section, options }) => {
    const [active, setActive] = react.useState(false);
    const [portalContainer, setPortalContainer] = react.useState(null);
    const pickerRef = react.useRef(null);
    const dropdownRef = react.useRef(null);

    // Function to check if the click is outside the dropdown
    const handleClickOutside = (event) => {
        if (pickerRef.current && !pickerRef.current.contains(event.target) &&
            dropdownRef.current && !dropdownRef.current.contains(event.target)) {
            setActive(false);
        }
    };

    react.useEffect(() => {
        // Create a single container for the portal if not already created
        if (!document.getElementById('select-portal')) {
            const container = document.createElement('div');
            container.id = 'select-portal';
            document.body.appendChild(container);
            setPortalContainer(container);
        } else {
            setPortalContainer(document.getElementById('select-portal'));
        }

        // Add event listener to close the dropdown when clicking outside
        document.addEventListener('click', handleClickOutside, true);

        // Cleanup the event listener
        return () => {
            document.removeEventListener('click', handleClickOutside, true);
        };
    }, []);

    react.useEffect(() => {
        // Toggle the click listener based on dropdown state
        if (active) {
            document.addEventListener('click', handleClickOutside, true);
        } else {
            document.removeEventListener('click', handleClickOutside, true);
        }
    }, [active]);

    const getDropdownPosition = () => {
        if (pickerRef.current) {
            const rect = pickerRef.current.getBoundingClientRect();
            return {
                top: rect.bottom + window.scrollY,
                left: rect.left + window.scrollX,
                width: rect.width
            };
        }
        return {};
    };

    const dropdownContent = active ? (
        <div ref={dropdownRef} style={{
            display: 'flex',
            position: 'absolute',
            ...getDropdownPosition(),
            zIndex: 9999,
        }}>
            <div className="dropdown-popup_mMv"
                style={{
                    maxWidth: 'inherit',
                    width: '100%',
                    maxHeight: '300px', // Set a maximum height
                    overflowY: 'auto' // Enable vertical scrolling
                }}>
                <div className="dropdown-menu_jf2 dropdown-menu_Swd"
                    style={{
                        maxWidth: 'inherit',
                        width: '100%',
                        maxHeight: '300px', // Set a maximum height
                        overflowY: 'auto' // Enable vertical scrolling
                    }}>
                    {Object.keys(options).map((option, index) => (
                        <div key={option} className="dropdown-item_sZT" style={{ padding: '5rem', height: 'auto' }}>
                            <$Field key={index}
                                react={react}
                                label={option}
                                checked={options[option]}
                                onToggle={onToggle(section, option)} />
                        </div>
                    ))}
                </div>
            </div>
        </div>
    ) : null;

    const selectedOptionsText = Object.keys(options).filter(key => options[key]).join(', ');

    return (
        <div style={{ width: '100%' }}>
            <div ref={pickerRef} className="dropdown-toggle_V9z dropdown-toggle_prl value-field_yJi value_PW_ dropdown_pJu item-states_QjV" onClick={() => setActive(!active)} style={{ padding: '5rem', height: 'auto', ...style }}>
                <div className="label_l_4">{selectedOptionsText || 'Select options'}</div>
                <div className="tinted-icon_iKo indicator_Xmj" style={{ maskImage: 'url(Media/Glyphs/StrokeArrowDown.svg)' }}></div>
                {portalContainer && dropdownContent && ReactDOM.createPortal(dropdownContent, portalContainer)}
            </div>
        </div>
    );
}

export default $Select;