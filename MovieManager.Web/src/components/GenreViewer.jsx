import "./GenreViewer.css"
import { useState, forwardRef, useImperativeHandle, useRef } from "react";
import { Pagination, Button, Spin, Modal, Descriptions, Input } from 'antd';
import { HeartFilled, HeartOutlined } from '@ant-design/icons';
import { getMoivesByFilter, getGenreByName, likeGenre } from "../services/DataService";
import MovieViewer from "./MovieViewer";
import { MOVIE_CARD_EACH_PAGE_SMALL_SCREEN } from "../Constant";

const { Search } = Input;

const GenreViewer = forwardRef((props, ref) => {
    const numEachPage = 63;

    const [minValue, setMinValue] = useState(0);
    const [maxValue, setMaxValue] = useState(numEachPage);
    const [genres, setGenres] = useState([]);
    const [genre, setGenre] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [visible, setVisible] = useState(false);
    const [isLikeLoading, setIsLikeLoading] = useState(false);
    const [likeFlag, setLikeFlag] = useState(0);

    const movieViewer = useRef();

    useImperativeHandle(ref, () => ({
        initializeGenres(genres) {
            init(genres);
        },
        setIsLoading() {
            setIsLoading(true);
        }
    }));

    function init(genres) {
        setMinValue(0);
        setMaxValue(numEachPage);
        setGenres(genres);
        setIsLoading(false);
    }

    function handleChange(value) {
        setMinValue((value - 1) * numEachPage);
        setMaxValue(value * numEachPage);
    };

    function showGenreDetails(genreIndex) {
        getGenreByName(genres[genreIndex]).then(resp => {
            setGenre(resp[0]);
            setVisible(true);
            movieViewer?.current.setIsLoading();
            setLikeFlag(resp[0].liked);
            getMoivesByFilter(1, [genres[genreIndex]], false).then(resp => {
                movieViewer?.current.initializeMovies(resp, MOVIE_CARD_EACH_PAGE_SMALL_SCREEN, genres[genreIndex]);
            });
        }).catch((error) => {
            console.log(error);
        });
    }

    function onSearch(value) {
        setIsLoading(true);
        getGenreByName(value).then(resp => {
            let genres = resp ? resp.map(x => x.name) : [];
            init(genres);
        }).catch(error => console.log(error));
    }

    function onLikeClick() {
        setIsLikeLoading(true);
        likeGenre(genre?.name).then(resp => {
            setIsLikeLoading(false);
            setLikeFlag(resp);
        }).catch(error => console.log(error));
    }

    return (
        <div className="genre-viewer">
            {isLoading ? <div><Spin size="large" /></div> :
                <Pagination
                    simple
                    defaultCurrent={1}
                    defaultPageSize={numEachPage} //default size of page
                    onChange={handleChange}
                    total={genres?.length}
                    className="header-left"
                />}
            <Search placeholder="类型名" onSearch={onSearch} className="header-right genre-search-bar" loading={isLoading} />
            {isLoading ? <div><Spin size="large" /></div> :
                <div>
                    <div className="genre-list">
                        {genres?.slice(minValue, maxValue).map((genre, i) =>
                            <Button key={"genre-" + i + minValue} className="genre-button" onClick={() => showGenreDetails(i + minValue)}>
                                <span className="genre-span">{genre}</span>
                            </Button>)}
                    </div>
                </div>}
            <Modal
                title={[<Button key="genre-like-btn"
                    shape="circle"
                    icon={likeFlag === true ? <HeartFilled /> : <HeartOutlined />}
                    onClick={onLikeClick}
                    loading={isLikeLoading}></Button>]}
                centered
                visible={visible}
                onOk={() => setVisible(false)}
                onCancel={() => setVisible(false)}
                width={1100}
                className="genre-details"
            >
                <Descriptions title={genre?.name} bordered>
                </Descriptions>
                <MovieViewer ref={movieViewer} searchString2={genre?.name} searchType="Genre"/>
            </Modal>
        </div>
    )
});
export default GenreViewer;