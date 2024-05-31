import "./TagViewer.css"
import { useState, forwardRef, useImperativeHandle, useRef } from "react";
import { Pagination, Button, Spin, Modal, Descriptions, Input } from 'antd';
import { HeartFilled, HeartOutlined } from '@ant-design/icons';
import { getMoivesByFilter, getTagByName, likeTag } from "../services/DataService";
import MovieViewer from "./MovieViewer";
import { MOVIE_CARD_EACH_PAGE_SMALL_SCREEN } from "../Constant";

const { Search } = Input;

const TagViewer = forwardRef((props, ref) => {
    const numEachPage = 63;

    const [minValue, setMinValue] = useState(0);
    const [maxValue, setMaxValue] = useState(numEachPage);
    const [tags, setTags] = useState([]);
    const [tag, setTag] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [visible, setVisible] = useState(false);
    const [isLikeLoading, setIsLikeLoading] = useState(false);
    const [likeFlag, setLikeFlag] = useState(0);

    const movieViewer = useRef();

    useImperativeHandle(ref, () => ({
        initializeTags(tags) {
            init(tags);
        },
        setIsLoading() {
            setIsLoading(true);
        }
    }));

    function init(tags) {
        setMinValue(0);
        setMaxValue(numEachPage);
        setTags(tags);
        setIsLoading(false);
    }

    function handleChange(value) {
        setMinValue((value - 1) * numEachPage);
        setMaxValue(value * numEachPage);
    };

    function showTagDetails(tagIndex) {
        getTagByName(tags[tagIndex]).then(resp => {
            setTag(resp[0]);
            setVisible(true);
            movieViewer?.current.setIsLoading();
            setLikeFlag(resp[0].liked);
            getMoivesByFilter(1, [tags[tagIndex]], false).then(resp => {
                movieViewer?.current.initializeMovies(resp, MOVIE_CARD_EACH_PAGE_SMALL_SCREEN, tags[tagIndex]);
            });
        }).catch((error) => {
            console.log(error);
        });
    }

    function onSearch(value) {
        setIsLoading(true);
        getTagByName(value).then(resp => {
            let tags = resp ? resp.map(x => x.name) : [];
            init(tags);
        }).catch(error => console.log(error));
    }

    function onLikeClick() {
        setIsLikeLoading(true);
        likeTag(tag?.name).then(resp => {
            setIsLikeLoading(false);
            setLikeFlag(resp);
        }).catch(error => console.log(error));
    }

    return (
        <div className="tag-viewer">
            {isLoading ? <div><Spin size="large" /></div> :
                <Pagination
                    simple
                    defaultCurrent={1}
                    defaultPageSize={numEachPage} //default size of page
                    onChange={handleChange}
                    total={tags?.length}
                    className="header-left"
                />}
            <Search placeholder="类型名" onSearch={onSearch} className="header-right tag-search-bar" loading={isLoading} />
            {isLoading ? <div><Spin size="large" /></div> :
                <div>
                    <div className="tag-list">
                        {tags?.slice(minValue, maxValue).map((tag, i) =>
                            <Button key={"tag-" + i + minValue} className="tag-button" onClick={() => showTagDetails(i + minValue)}>
                                <span className="tag-span">{tag}</span>
                            </Button>)}
                    </div>
                </div>}
            <Modal
                title={[<Button key="tag-like-btn"
                    shape="circle"
                    icon={likeFlag === true ? <HeartFilled /> : <HeartOutlined />}
                    onClick={onLikeClick}
                    loading={isLikeLoading}></Button>]}
                centered
                visible={visible}
                onOk={() => setVisible(false)}
                onCancel={() => setVisible(false)}
                width={1100}
                className="tag-details"
            >
                <Descriptions title={tag?.name} bordered>
                </Descriptions>
                <MovieViewer ref={movieViewer} searchString2={tag?.name} searchType="Tag"/>
            </Modal>
        </div>
    )
});
export default TagViewer;