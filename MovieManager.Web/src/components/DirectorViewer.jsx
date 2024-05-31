import "./DirectorViewer.css"
import { useState, forwardRef, useImperativeHandle, useRef } from "react";
import { Pagination, Button, Spin, Modal, Descriptions, Input } from 'antd';
import { getMoivesByFilter } from "../services/DataService";
import MovieViewer from "./MovieViewer";

const directorViewer = forwardRef((props, ref) => {
    const numEachPage = 63;

    const [minValue, setMinValue] = useState(0);
    const [maxValue, setMaxValue] = useState(numEachPage);
    const [directors, setDirectors] = useState([]);
    const [director, setDirector] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [visible, setVisible] = useState(false);

    const movieViewer = useRef();

    useImperativeHandle(ref, () => ({
        initializeDirectors(directors) {
            init(directors);
        },
        setIsLoading() {
            setIsLoading(true);
        }
    }));

    function init(directors) {
        setMinValue(0);
        setMaxValue(numEachPage);
        setDirectors(directors);
        setIsLoading(false);
    }

    function handleChange(value) {
        setMinValue((value - 1) * numEachPage);
        setMaxValue(value * numEachPage);
    };

    function showDirectorDetails(directorIndex) {
        setDirector(directors[directorIndex]);
        setVisible(true);
        // movieViewer?.current.setIsLoading();
        getMoivesByFilter(3, [directors[directorIndex]], false).then(resp => {
            movieViewer?.current.initializeMovies(resp, 5, directors[directorIndex]);
        });
    }

    // function onSearch(value) {
    //     setIsLoading(true);
    //     getdirectorByName(value).then(resp => {
    //         let directors = resp ? resp.map(x => x.name) : [];
    //         init(directors);
    //     }).catch(error => console.log(error));
    // }

    return (
        <div className="director-viewer">
            {isLoading ? <div><Spin size="large" /></div> :
                <Pagination
                    simple
                    defaultCurrent={1}
                    defaultPageSize={numEachPage} //default size of page
                    onChange={handleChange}
                    total={directors?.length}
                    className="header-left"
                />}
            {/* <Search placeholder="导演名" onSearch={onSearch} className="header-right director-search-bar" loading={isLoading} /> */}
            {isLoading ? <div><Spin size="large" /></div> :
                <div>
                    <div className="director-list">
                        {directors?.slice(minValue, maxValue).map((director, i) =>
                            <Button key={"director-" + i + minValue} className="director-button" onClick={() => showDirectorDetails(i + minValue)}>
                                <span className="director-span">{director}</span>
                            </Button>)}
                    </div>
                </div>}
            <Modal
                title={[<Button key="director-like-btn"
                    shape="circle"></Button>]}
                centered
                visible={visible}
                onOk={() => setVisible(false)}
                onCancel={() => setVisible(false)}
                width={1100}
                className="director-details"
            >
                <Descriptions title={director?.name} bordered>
                </Descriptions>
                <MovieViewer ref={movieViewer} />
            </Modal>
        </div>
    )
});
export default directorViewer;