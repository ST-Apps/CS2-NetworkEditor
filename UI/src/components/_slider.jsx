import React from 'react';

const $Slider = ({ react, value, onValueChanged, style }) => {
    const sliderRef = react.useRef(null); // Reference to the slider element

    const handleSliderClick = (e) => {
        const slider = sliderRef.current;

        if (!slider)
            return;

        // Get the click position relative to the slider
        const rect = slider.getBoundingClientRect();
        const clickedPosition = e.clientX - rect.left;

        // Calculate the new value based on click position
        let newValue = (clickedPosition / rect.width) * 100;

        // Round to the nearest multiple of 5
        newValue = Math.round(newValue / 5) * 5;
        newValue = parseInt(newValue, 10);

        if (newValue < 0)
            newValue = 0;
        else if (newValue > 100)
            newValue = 100;

        if (onValueChanged)
            onValueChanged(parseInt(newValue, 10));
    };

    const valuePercent = value + "%";
    return (
        <div style={{ width: '100%', ...style }}>
            <div style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', justifyContent: 'center', margin: '10rem', marginTop: '0' } }>
                <div className="value_jjh" style={{ display: 'flex', width: '45rem', alignItems: 'center', justifyContent: 'center' }}>{valuePercent}</div>
                <div
                    className="slider_fKm slider_pUS horizontal slider_KjX"
                    style={{
                        flex: 1,
                        margin: '10rem',
                    }}
                    ref={sliderRef}
                    onClick={handleSliderClick}
                    >
                    <div className="track-bounds_H8_">
                        <div className="range-bounds_lNt" style={{ width: valuePercent }}>
                            <div className="range_KXa range_iUN"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default $Slider;
