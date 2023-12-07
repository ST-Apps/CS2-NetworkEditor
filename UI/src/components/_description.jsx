import React from 'react'

const $Description = ({ style, children }) => {
    return <div className="description_VWf" style={{ padding: '20rem', ...style }}>
        <div className="paragraphs_nbD">
            {children}
        </div>
    </div>

}

export default $Description