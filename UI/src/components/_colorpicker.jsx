import React from 'react';
import ReactDOM from 'react-dom';
import $FancySlider from '../components/_fancy-slider';

const $ColorPicker = ({ react, label, color, onChanged }) => {
    function hsvToColor(h, s, v, alpha = 1) {
        let r, g, b;

        let i = Math.floor(h * 6);
        let f = h * 6 - i;
        let p = v * (1 - s);
        let q = v * (1 - f * s);
        let t = v * (1 - (1 - f) * s);

        switch (i % 6) {
            case 0: r = v, g = t, b = p; break;
            case 1: r = q, g = v, b = p; break;
            case 2: r = p, g = v, b = t; break;
            case 3: r = p, g = q, b = v; break;
            case 4: r = t, g = p, b = v; break;
            case 5: r = v, g = p, b = q; break;
        }

        return `rgba(${Math.round(r * 255)}, ${Math.round(g * 255)}, ${Math.round(b * 255)}, ${alpha})`;
    }

    function hexToHsv(hex) {
        // Remove the hash at the start if it's there
        hex = hex.replace(/^#/, '');

        // Parse the hex string
        let r, g, b;
        if (hex.length === 3) {
            r = parseInt(hex[0] + hex[0], 16);
            g = parseInt(hex[1] + hex[1], 16);
            b = parseInt(hex[2] + hex[2], 16);
        } else if (hex.length === 6) {
            r = parseInt(hex.substring(0, 2), 16);
            g = parseInt(hex.substring(2, 4), 16);
            b = parseInt(hex.substring(4, 6), 16);
        } else {
            throw new Error('Invalid hex color: ' + hex);
        }

        // Convert RGB to HSV
        r /= 255;
        g /= 255;
        b /= 255;

        let max = Math.max(r, g, b), min = Math.min(r, g, b);
        let h, s, v = max;

        let d = max - min;
        s = max === 0 ? 0 : d / max;

        if (max == min) {
            h = 0; // achromatic
        } else {
            switch (max) {
                case r: h = (g - b) / d + (g < b ? 6 : 0); break;
                case g: h = (b - r) / d + 2; break;
                case b: h = (r - g) / d + 4; break;
            }
            h /= 6;
        }

        return { h, s, v };
    }

    function hsvToHex(h, s, v) {
        let r, g, b;

        let i = Math.floor(h * 6);
        let f = h * 6 - i;
        let p = v * (1 - s);
        let q = v * (1 - f * s);
        let t = v * (1 - (1 - f) * s);

        switch (i % 6) {
            case 0: r = v, g = t, b = p; break;
            case 1: r = q, g = v, b = p; break;
            case 2: r = p, g = v, b = t; break;
            case 3: r = p, g = q, b = v; break;
            case 4: r = t, g = p, b = v; break;
            case 5: r = v, g = p, b = q; break;
        }

        // Convert to hex string
        const toHex = c => {
            const hex = Math.round(c * 255).toString(16);
            return hex.length === 1 ? '0' + hex : hex;
        };

        return `#${toHex(r)}${toHex(g)}${toHex(b)}`;
    }


    const hsv = hexToHsv(color);
    const hue = hsv.h;
    const saturation = hsv.s;
    const value = hsv.v;

    const curColour = hsvToColor(hue, saturation, value);

    const [active, setActive] = react.useState(false);
    const [portalContainer, setPortalContainer] = react.useState(null);
    const pickerRef = react.useRef(null); // Ref to attach to the color picker field
    const dropdownRef = react.useRef(null); // Ref for the dropdown content

    // Function to check if the click is outside the dropdown
    const handleClickOutside = (event) => {
        if (pickerRef.current && !pickerRef.current.contains(event.target) &&
            dropdownRef.current && !dropdownRef.current.contains(event.target)) {
            setActive(false);
        }
    };

    react.useEffect(() => {
        // Create a single container for the portal if not already created
        if (!document.getElementById('color-picker-portal')) {
            const container = document.createElement('div');
            container.id = 'color-picker-portal';
            document.body.appendChild(container);
            setPortalContainer(container);
        } else {
            setPortalContainer(document.getElementById('color-picker-portal'));
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

    const onToggle = () => {
        setActive(!active);
    };

    const hueVal = parseInt(hue * 100, 10);
    const satVal = parseInt(saturation * 100, 10);
    const valVal = parseInt(value * 100, 10);

    // For the Saturation slider, create a gradient from white to the full saturation of the current hue
    const satFromColour = hsvToColor(hue, 0, 1); // White
    const satToColour = hsvToColor(hue, 1, 1);   // Full saturation

    // For the Value slider, create a gradient from black to the current hue at full brightness
    const valFromColour = hsvToColor(hue, saturation, 0); // Black
    const valToColour = hsvToColor(hue, saturation, 1);   // Full brightness


    const onHueUpdated = (val) => {
        let newColor = hsvToHex(val / 100.0, saturation, value);
        onChanged(newColor);
    };

    const onSatUpdated = (val) => {
        let newColor = hsvToHex(hue, val / 100.0, value);
        onChanged(newColor);

    };
    const onValUpdated = (val) => {
        let newColor = hsvToHex(hue, saturation, val / 100.0);
        onChanged(newColor);
    };

    // Define the dropdown content
    const dropdownContent = active ? (
        <div ref={dropdownRef} style={{
            display: 'flex',
            position: 'absolute',
            ...getDropdownPosition(),
            zIndex: 9999
        }}>
            <div className="color-picker-container_Sj5" style={{ maxWidth: 'inherit', 'width': '100%' }}>
                <div className="color-picker_aNX">
                    <div className="sliders_sCL section_cwE">
                        <div className="color-component-input_WeK" style={{ flexDirection: 'column', alignItems: 'stretch', justifyContent: 'stretch' }}>
                            <$FancySlider react={react} value={hueVal} isColorSpectrum="true" onValueChanged={onHueUpdated} /> 
                            <$FancySlider react={react} value={satVal} fromColour={satFromColour} toColour={satToColour} onValueChanged={onSatUpdated} style={{ marginTop: '5rem' }} />
                            <$FancySlider react={react} value={valVal} fromColour={valFromColour} toColour={valToColour} onValueChanged={onValUpdated} style={{ marginTop: '5rem' }} />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    ) : null;

    return (
        <div style={{ position: 'relative' }}>
            <div ref={pickerRef} className="field_amr field_cjf" style={{ display: 'flex', flexDirection: 'row' }} onClick={onToggle}>
                <div style={{ flex: 1 }} onClick={onToggle}>
                    {label}
                </div>
                <div className="color-field_jwA color-field_due" style={{ marginLeft: 'auto' }}>
                    <div style={{ backgroundColor: curColour }}>
                    </div> 
                </div>
            </div>
            {portalContainer && dropdownContent && ReactDOM.createPortal(dropdownContent, portalContainer)}
        </div>
    );
}

export default $ColorPicker;
